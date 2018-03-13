using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// Delivery checks are a set of tests which can help find out potential issues related to ad delivery.
    /// </summary>
    public class DeliveryChecks
    {
        [JsonProperty("check_name")]
        public string CheckName { get; set; }
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
