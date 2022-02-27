using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Get_Requests_From_Client_For_Project_Test
{
    /// <summary>
    /// Method response.
    /// </summary>
    public class ActionResponse
    {
        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>
        /// The answer.
        /// </value>
        public Result Answer { get; set; }
        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        /// <value>
        /// The request id.
        /// </value>
        public string RequestId { get; set; }
        /// <summary>
        /// Gets or sets the value. If answer is not <c>OK</c>, value is null; otherwise <c>double</c>.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public double? Value { get; set; }
        /// <summary>
        /// Converts this to object.
        /// </summary>
        /// <returns>The object.</returns>
        public object ToObject()
        {
            return new { Answer, RequestId, Value };
        }
    }

    /// <summary>
    /// Answer result.
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// The OK.
        /// </summary>
        OK,
        /// <summary>
        /// The fatal error.
        /// </summary>
        ERROR,
        /// <summary>
        /// The error cause by wrong algorithm
        /// </summary>
        ERROR_WRONG_ALGORITHM,
        /// <summary>
        /// The error cause by wrong dataset.
        /// </summary>
        ERROR_WRONG_DATASET,
        /// <summary>
        /// The error cause by wrong requestorid.
        /// </summary>
        ERROR_WRONG_REQUEST_ID
    }
}
