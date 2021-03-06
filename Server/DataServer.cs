using Get_Requests_From_Client_For_Project_Test.AdditionalMethods;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Get_Requests_From_Client_For_Project_Test.Server;
using Get_Requests_From_Client_For_Project_Test.RabbitMQRPCClient;
using Get_Requests_From_Client_For_Project_Test.DataSetsClasses;

namespace Get_Requests_From_Client_For_Project_Test
{
    /// <summary>
    /// The data server.
    /// </summary>
    public class DataServer
    {
        /// <summary>
        /// The data server constructor.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="config">The configuration.</param>
        public DataServer(Logger logger, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;

            _isRabbitMQ = config["UseRabbitMQ"].ConvertTo<bool>();
            _rabbitMQSettings = this.config.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

            if (!_isRabbitMQ)
            {
                _allowedIPs = new();
                _listAllowedMachines = new();
                _listAllowedMachinesEmployment = new();
                UpdateAllowedIps();
                _timer = new(UpdateListAllowedMachines);
                _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds
                    (
                    string.IsNullOrEmpty(config["CheckAllowedLocalMachinesPeriod"].ToString()) ?
                        30 :
                        int.Parse(config["CheckAllowedLocalMachinesPeriod"].ToString())
                    ));
            }
            else
            {
                logger.Info("Starts with RabbitMQ configuration: {@rabbitMQsettings}", _rabbitMQSettings);
                _rpcClient = new(_rabbitMQSettings, this.logger);
            }
        }

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = false,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly Logger logger;
        private readonly IConfiguration config;

        private readonly Timer _timer;

        private readonly bool _isRabbitMQ;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly RpcClientRabbitMQ _rpcClient;

        // List of allowed ips of local machines.
        private List<string> _allowedIPs;
        //Dictionary to see connected local machines.
        private ConcurrentDictionary<string, bool> _listAllowedMachines;
        //Dictionary of allowed machines employment.
        private ConcurrentDictionary<string, long> _listAllowedMachinesEmployment;
        /// <summary>
        /// Method uses to post request to least busy local machine.
        /// </summary>
        /// <param name="algorithm">The AI algorithm.</param>
        /// <param name="data">The data.</param>
        /// <returns>ActionResultReponse</returns>
        public ActionResponse HttpRequestToCalc(string algorithm, object data)
        {
            logger.Info("T RequestToCalc<T, D>(string {algorithm}, object {@data}", algorithm, data);
            logger.Debug("Current list of allowed machines employment: {@_listAllowedMachinesEmployment}", _listAllowedMachinesEmployment);
            string leastBusyMachineIp = _listAllowedMachinesEmployment.GetLeastBusyMachine().Key;
            if (!String.IsNullOrEmpty(leastBusyMachineIp))
            {
                _listAllowedMachinesEmployment[leastBusyMachineIp]++;
                (bool exec, bool res, string obj) = HttpRequest(leastBusyMachineIp, algorithm, HttpRequestType.Post, data).Result;
                _listAllowedMachinesEmployment[leastBusyMachineIp]--;
                if (exec && res && !String.IsNullOrEmpty(obj))
                    return JsonSerializer.Deserialize<ActionResponse>(obj, jsonSerializerOptions);
                else
                {
                    return new ActionResponse()
                    {
                        Answer = Result.ERROR
                    };
                }
            }
            return default;
        }

        public ActionResponse RequestToCalc(string algorithm, BaseRequest data)
        {
            if (_isRabbitMQ)
            {
                return RabbitRequestToCalc(algorithm, data);
            }
            else
            {
                return HttpRequestToCalc(algorithm, data);
            }
        }

        public ActionResponse RabbitRequestToCalc(string algorithm, BaseRequest data)
        {
            try
            {
                logger.Info("Request by rabbitMQ {alg} with data {@data}", algorithm, data);
                ActionResponse result = _rpcClient.Call(data);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    throw new Exception("Empty request result!");
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error on posting Rpc rabbit request!", ex.Message);
                return new ActionResponse()
                {
                    Answer = Result.ERROR
                };
            }
        }

        #region old_RPC_realization
        /*
        /// <summary>
        /// Method uses to post RPC request to local machine.
        /// </summary>
        /// <typeparam name="T">The response data type.</typeparam>
        /// <typeparam name="D">The data type.</typeparam>
        /// <param name="url">The url.</param>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="route">The rpc route.</param>
        /// <returns>Deserialized RPC response.</returns>
        private async Task<T> PostRPCRequest<T, D>(string url, string route, string algorithm, D data)
        {
            try
            {
                logger.Info("private async Task<T> PostRPCRequest<T, D>(string {url}, string {algorithm}, object {@data})", url, algorithm, data);
                UriBuilder builder = new(url);

                //ЧЕРТОВ КОСТЫЛЬ!
                Newtonsoft.Json.JsonSerializerSettings jsonSerializerSettings = new()
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                };
                jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                //Подумать как этого избежать и использовать microsoft json
                RpcClient client = new(builder.Uri, new DefaultRequestJsonSerializer(jsonSerializerSettings: jsonSerializerSettings));
                RpcRequest request = new(Guid.NewGuid().ToString(), algorithm, RpcParameters.From(data));
                logger.Info("Rpc request to url {url} with params: {@params}", client.BaseUrl, request);
                RpcResponse response = await client.SendRequestAsync(request, route, typeof(T));
                logger.Info("Answer is {@response}", response);
                if (!response.HasError)
                {
                    T respResult = (T)response.Result;
                    logger.Info("Rpc response is {@response}", respResult);
                    return respResult;
                }
                else
                {
                    logger.Error("Response contains error!");
                    return default;
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error on sending RPC request", ex.Message);
                return default;
            }
        }*/
        #endregion

        /// <summary>
        /// Method uses to update list of allowed local machines.
        /// </summary>
        private async void UpdateListAllowedMachines(object state)
        {
            logger.Info("Current list of allowed local machines: {@list}", _listAllowedMachines);
            _listAllowedMachines = new();
            _listAllowedMachinesEmployment = new();
            foreach (string ip in _allowedIPs)
            {
                (bool exec, bool res) = await Ping(ip);
                _listAllowedMachines[ip] = res && exec;
                if (!_listAllowedMachinesEmployment.TryGetValue(ip, out _) && (res && exec))
                {
                    _listAllowedMachinesEmployment[ip] = 0L;
                }
            }
            logger.Info("Updated list of allowed local machines: {@list}", _listAllowedMachines);
        }

        /// <summary>
        /// Method uses to update list of allowed ips of local machines.
        /// </summary>
        /// <param name="newIps">The new ips. Uses on hard set ips.</param>
        private void UpdateAllowedIps(List<string> newIps = null)
        {
            if (newIps != null)
            {
                _allowedIPs = newIps;
            }
            else
            {
                _allowedIPs = new();
                logger.Info("Current list of allowed ips of local machines: {@allowedIps}", _allowedIPs);
                foreach (IConfigurationSection val in config.GetSection("AllowedIPs").GetChildren().ToArray())
                {
                    _allowedIPs.Add(val.Value);
                }
                logger.Info("Updated list of allowed ips of local machines: {@allowedIps}", _allowedIPs);
            }
        }

        /// <summary>
        /// Usses to get ping from local machine.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns>exec - <c>true</c> if executed; otherwise <c>false</c>| <c>true</c> if http response status code is 200; otherwise <c> false</c></returns>
        public async Task<(bool exec, bool res)> Ping(string ip)
        {
            (bool exec, bool res, string _) = await HttpRequest(ip, "ping", HttpRequestType.Get);
            return (exec, res);
        }

        /// <summary>
        /// Method uses to post http requests.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="method">The method.</param>
        /// <param name="requestType">The request type.</param>
        /// <param name="data">The data.</param>
        /// <returns>exec - <c>true</c> if executed; otherwise <c>false</c>| <c>true</c> if http response has 200 status code; otherwise <c> false</c></returns>
        public async Task<(bool exec, bool res, string obj)> HttpRequest(string url, string method, HttpRequestType requestType, object data = null)
        {
            try
            {
                string json = JsonSerializer.Serialize(data, jsonSerializerOptions);
                UriBuilder builder = new($"{url}/{method}");
                using HttpClientHandler handler = new();
                using HttpClient httpClient = new(handler);
                HttpResponseMessage response = new();
                logger.Info("Http request: request type {requestType}, url {uri} , json {json}", requestType, builder.Uri, json);
                switch (requestType)
                {
                    case HttpRequestType.Get:
                        response = await httpClient.GetAsync(builder.Uri).ConfigureAwait(false);
                        break;
                    case HttpRequestType.Post:
                        if (json != null)
                        {
                            response = await httpClient.PostAsync(builder.Uri, new StringContent(json, Encoding.UTF8, "application/json"));
                        }
                        break;
                    default:
                        logger.Error("Wrong request type {requestType}", requestType);
                        return (false, false, null);
                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string answer = await response.Content.ReadAsStringAsync();
                    logger.Info("Answer is {answer}", answer);
                    return (true, true, answer);
                }
                else
                {
                    logger.Info("Answer status code is {response.StatusCode}", response.StatusCode);
                    return (true, true, null);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error on post request", ex.Message);
                return (false, false, null);
            }
        }
    }

    /// <summary>
    /// The http request type.
    /// </summary>
    public enum HttpRequestType
    {
        /// <summary>
        /// To get http request.
        /// </summary>
        Get,
        /// <summary>
        /// To post http request.
        /// </summary>
        Post
    }
}
