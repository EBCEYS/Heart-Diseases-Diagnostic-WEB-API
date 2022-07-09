using Get_Requests_From_Client_For_Project_Test.DataSetsClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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

        }
        private readonly Logger logger;
        private readonly IConfiguration config;
        private readonly DataServer _server;


        /// <summary>
        /// The main method. Uses to diagnose data by definite algorithm.
        /// </summary>
        /// <param name="algorithm">The AI algorithm.</param>
        /// <param name="dataSetType">The data set type.</param>
        /// <param name="data">The values set by dataset example.</param>
        /// <param name="requestId">The request id.</param>
        /// <returns>The action response.</returns>
        [ProducesResponseType(typeof(ActionResponse), 200)]
        [HttpPost("{algorithm}/{dataSetType}/diagnose")]
        public ActionResult<ActionResponse> Diagnose([Required][FromRoute] AlgorithmsTypes algorithm, [Required][FromRoute] DataSetTypes dataSetType, [FromBody] JsonDocument data, [Required][FromHeader] string requestId)
        {
            logger.Info("public ActionResult<ActionResponse> Diagnose([Required][FromQuery] AlgorithmsTypes {algorithm}, [Required][FromQuery] DataSetTypes {dataSetType}, [FromBody] JsonDocument {@data}, [Required][FromHeader] string {requestId})", algorithm, dataSetType, data.RootElement.ToString(), requestId);
            string dataSet = data.RootElement.ToString();
            ActionResponse response;
            if (requestId == null)
            {
                response = new() { Answer = Result.ERROR_WRONG_REQUEST_ID };
                return Ok(response);
            }
            try
            {
                switch (dataSetType)
                {
                    case DataSetTypes.Cleveland:
                        ClevelandDataSet clevelandDataSet = JsonSerializer.Deserialize<ClevelandDataSet>
                            (dataSet,
                            new JsonSerializerOptions() 
                            { 
                                WriteIndented = false, 
                                AllowTrailingCommas = true, 
                                PropertyNameCaseInsensitive = true 
                            });
                        if (clevelandDataSet.CheckAttributes(out List<string> nullVals))
                        {
                            BaseRequest request = new()
                            {
                                Params = clevelandDataSet,
                                Method = algorithm.ToString()
                            };
                            response = _server.RequestToCalc(algorithm.ToString(), request);
                            if (response != null)
                            {
                                response.RequestId = requestId;
                            }
                        }
                        else
                        {
                            logger.Error("Null values list: {@nullVals}", nullVals);
                            response = new() { Answer = Result.ERROR_WRONG_DATASET, RequestId = requestId, Value = null };
                            break;
                        }
                        break;
                    default:
                        response = new() { Answer = Result.ERROR_WRONG_DATASET, RequestId = requestId, Value = null };
                        break;
                }
            }
            catch (JsonException ex)
            {
                logger.Error(ex, "Parsing error", ex.Message);
                response = null;
            }
            if (response == null)
            {
                response = new() { Answer = Result.ERROR, RequestId = requestId, Value = null };
            }
            logger.Info("Response is {@response}", response);
            return Ok(response);
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

        /// <summary>
        /// Get cleveland data set example.
        /// </summary>
        /// <returns>The cleveland data set example.</returns>
        [ProducesResponseType(typeof(ClevelandDataSet), 200)]
        [HttpGet("/cleveland-example")]
        public ActionResult<ClevelandDataSet> GetClevelandExample()
        {
            logger.Info("public ActionResult GetClevelandExample()");
            ClevelandDataSet example = new()
            {
                Age = 63,
                Sex = 1,
                ChestPainType = 3,
                RestingBloodPressure = 145,
                SerumCholestoral = 233,
                FastingBloodSugar = false,
                MaximumHeartRateAchieved = 150,
                RestingElectrocardiographicResults = 1,
                ExerciseInducedAngina = false,
                STDepression = 2.3,
                STSlope = 0,
                NumberOfMajorvessels = 0,
                Thalassemia = 1
            };
            example.SetAlghorithmType(null);
            logger.Info("Result is {@example}", example);
            return example;
        }
    }
}
