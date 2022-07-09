using System;
using System.Collections.Concurrent;
using System.Text;
using Get_Requests_From_Client_For_Project_Test.Server;
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
        private readonly BlockingCollection<string> respQueue = new();
        private readonly IBasicProperties props;
        private readonly RabbitMQSettings _rabbitMQSettings;

        public RpcClientRabbitMQ(RabbitMQSettings _rabbitMQSettings)
        {
            this._rabbitMQSettings = _rabbitMQSettings;
            ConnectionFactory factory = new() 
            { 
                HostName = _rabbitMQSettings.HostName, 
                Port = _rabbitMQSettings.Port,
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password
            };
            /*ConnectionFactory factory = new()
            {
                Uri = new Uri("amqp://guest:guest@localhost:55006/")
            };*/
            /*ConnectionFactory factory = new()
            {
                HostName = "localhost",
                UserName = ConnectionFactory.DefaultUser,
                Password = ConnectionFactory.DefaultPass,
                Port = AmqpTcpEndpoint.UseDefaultPort
            };*/

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            string correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);
        }

        public string Call(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: _rabbitMQSettings.Queue,
                basicProperties: props,
                body: messageBytes);

            return respQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
