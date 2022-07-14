using System;
using System.Text.Json.Serialization;
using System.Threading;

namespace Get_Requests_From_Client_For_Project_Test.DataSetsClasses
{
    public class BaseRequest
    {
        public string Id { get; set; }
        public object Params { get; set; }
        [JsonIgnore]
        public ManualResetEvent Ev { get; set; }
        public string Jsonrpc { get; set; }
        public string Method { get; set; }

        public BaseRequest()
        {
            Jsonrpc = "2.0";
            Id = Guid.NewGuid().ToString();
        }

    }
}
