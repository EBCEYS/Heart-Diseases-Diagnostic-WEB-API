using Get_Requests_From_Client_For_Project_Test.DataSetsClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

/*
 * TODO:
 * 0) Стырить FormatRequest и FormatResponse у Козлова. Очень удобная штука для дебага.
 * 1) Подогнать enum-ы и классы под используемые.
 * 2) Придумать адекватные варианты HTTP запросов на этот рест.
 * 3) Придумать на чем писать AI. Пока из вариантов использовать полностью для всего .net .
 * Возможно можно будет написать RPC сервер на питоне (с точки зрения логики это адекватнее, но можно провести дополнительное исследование на производительность).
 * Если речь зашла о производительности, то мб плюсы?
 * 4) Создать RPC сервер для расчетов by AI так сказать.
 * 5) Подумать над дополнительными полям в конфигурационном файле.
 * 6) Написать прогу, которая бы автоматизировала стрес тесты (по идее не сложно наверное).
 */

namespace Get_Requests_From_Client_For_Project_Test.Controllers
{
    /// <summary>
    /// The heart disease controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HeartDiseaseController : ControllerBase
    {
        /// <summary>
        /// The heart desease controller constructor.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dataServer">The data server.</param>
        /// <param name="config">The configuration.</param>
        public HeartDiseaseController(Logger logger, DataServer dataServer, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
            this._server = dataServer;

            _RequestIdLength = Guid.NewGuid().ToString().Length;
        }
        private readonly Logger logger;
        private readonly IConfiguration config;
        private readonly DataServer _server;

        private readonly long _RequestIdLength;


        /// <summary>
        /// The main method. Uses to diagnose data by definite algorithm.
        /// </summary>
        /// <param name="algorithm">The AI algorithm.</param>
        /// <param name="dataSetType">The data set type.</param>
        /// <param name="data">The values set by dataset example.
        /// <example>
        /// ClevelandDataSet:
        /// {
        /// "RequestId":"6c6135c2-6c5c-460d-81c8-35316d0144dd",
        /// "Age":50,
        /// "Sex":true,
        /// "MaxHeartRate":140
        /// }
        /// </example></param>
        /// <returns>The action response.</returns>
        [ProducesResponseType(typeof(object), 200)]
        [HttpPost("/diagnose")]
        public ActionResult Diagnose([Required][FromQuery] AlgorithmsTypes algorithm, [Required][FromQuery] DataSetTypes dataSetType, [FromBody] JsonDocument data)
        {
            logger.Info("public ActionResult<ActionResponse> Get([Required][FromQuery] string {@algorithm}, [Required][FromBody] ClevelandDataSet {@dataSet})", algorithm, data.RootElement.ToString());
            string dataSet = data.RootElement.ToString();
            //TODO: сделать проверку на соответствие формату или вообще убрать RequestId
            string requestId = data.RootElement.GetProperty("RequestId").ToString().Length == _RequestIdLength ? data.RootElement.GetProperty("RequestId").ToString() : null;
            ActionResponse response = null;
            if (requestId == null)
            {
                response = new() { Answer = Result.ERROR_WRONG_REQUEST_ID };
                return Ok(response.ToObject());
            }
            logger.Debug("Body is: {@ds}", dataSet);
            try
            {
                switch (dataSetType)
                {
                    case DataSetTypes.Cleveland:
                        ClevelandDataSet clevelandDataSet = JsonSerializer.Deserialize<ClevelandDataSet>(dataSet, new() { WriteIndented = false, AllowTrailingCommas = true, PropertyNameCaseInsensitive = true });
                        response = _server.RequestToCalc<ActionResponse, ClevelandDataSet>(algorithm.ToString(), clevelandDataSet);
                        if (response != null)
                        {
                            response.RequestId = requestId;
                        }
                        break;
                    default:
                        response = new() { Answer = Result.ERROR_WRONG_DATASET, RequestId = requestId, Value = null };
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error on sending RPC request", ex.Message);
                response = null;
            }
            if (response == null)
            {
                response = new() { Answer = Result.ERROR, RequestId = requestId, Value = null };
            }
            logger.Info("Response is {@response}", response);
            return Ok(response.ToObject());
        }

        /// <summary>
        /// The ping.
        /// </summary>
        /// <returns>Pong</returns>
        [ProducesResponseType(typeof(string), 200)]
        [HttpGet("/ping")]
        public ActionResult Ping()
        {
            logger.Debug("public ActionResult Ping()");
            return Ok("Pong");
        }
    }
}
