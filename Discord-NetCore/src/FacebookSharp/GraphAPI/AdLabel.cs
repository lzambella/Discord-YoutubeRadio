using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// An AdLabel
    /// </summary>
    public class AdLabel
    {
        /// <summary>
        /// Ad label ID
        /// </summary>
        [JsonProperty("id")]
        string Id { get; set; }
        [JsonProperty("account")]
        public AdAccount Account { get; set; }
        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("updated_time")]
        public string UpdatedTime { get; set; }
    }
}
