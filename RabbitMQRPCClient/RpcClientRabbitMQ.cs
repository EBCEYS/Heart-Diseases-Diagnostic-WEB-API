using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Get_Requests_From_Client_For_Project_Test.DataSetsClasses;
using Get_Requests_From_Client_For_Project_Test.Reponse;
using Get_Requests_From_Client_For_Project_Test.Server;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Get_Requests_From_Client_For_Project_Test.RabbitMQRPCClient
{
    public class RpcClientRabbitMQ
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly ConcurrentDictionary<string, BaseResponse> respDitct= new();
        private readonly ConcurrentDictionary<string, BaseRequest> reqDict = new();
        private readonly IBasicProperties props;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly Logger logger;

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = false,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public RpcClientRabbitMQ(RabbitMQSettings _rabbitMQSettings, Logger logger)
        {
            this.logger = logger;
            this._rabbitMQSettings = _rabbitMQSettings;
            ConnectionFactory factory = new()
            {
                HostName = _rabbitMQSettings.HostName,
                Port = _rabbitMQSettings.Port,
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            string correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            channel.QueueDeclare(queue: _rabbitMQSettings.Queue, durable: false,
              exclusive: false, autoDelete: false, arguments: null);

            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string encodedMessage = Encoding.UTF8.GetString(body);
                BaseResponse response = null;
                this.logger.Info("Get rabbit response: {encodedMessage}", encodedMessage);
                try
                {
                    response = JsonSerializer.Deserialize<BaseResponse>(encodedMessage, jsonSerializerOptions);
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, $"Error on parsing encoded message!{Environment.NewLine}{encodedMessage}", ex.Message);
                }
                finally
                {
                    if (response != null)
                    {
                        if (reqDict.TryGetValue(response?.Id, out BaseRequest request) && ea.BasicProperties.CorrelationId == correlationId)
                        {
                            respDitct.TryAdd(response.Id, response);
                            request.Ev.Set();
                        }
                    }
                    else
                        this.logger.Error("Response is null!");
                }
            };

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);
        }

        public ActionResponse Call(BaseRequest data)
        {
            string message = JsonSerializer.Serialize(data, jsonSerializerOptions);
            data.Ev = new(false);
            reqDict.TryAdd(data.Id, data);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: _rabbitMQSettings.Queue,
                basicProperties: props,
                body: messageBytes);
            data.Ev.WaitOne(TimeSpan.FromSeconds(_rabbitMQSettings.Timeout));
            respDitct.TryRemove(data.Id, out BaseResponse result);
            return result?.Result ?? null;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
