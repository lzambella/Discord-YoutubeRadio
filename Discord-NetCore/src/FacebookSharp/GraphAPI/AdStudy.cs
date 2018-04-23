using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    public class AdStudy
    {
        [JsonProperty("business")]
        public Business Business { get; set; }
        [JsonProperty("canceled_time")]
        public string CanceledTime { get; set; }
        [JsonProperty("created_by")]
        public User CreatedBy { get; set; }
        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("end_time")]
        public string EndTime { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("cooldown_start_time")]
        public string CooldownStartTime { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("updated_by")]
        public User UpdatedBy { get; set; }
        //todo:
        [JsonProperty("update_time")]
        public string UpdateTime { get; set; } 
    }
}
