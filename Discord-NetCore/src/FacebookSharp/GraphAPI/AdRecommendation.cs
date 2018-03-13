using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace FacebookSharp.GraphAPI
{
    public class AdRecommendation
    {
        //TODO: not the best name
        public enum Level
        {
            HIGH, MEDIUM, LOW
        }
        [JsonProperty("blame_field")]
        public string BlameField { get; set; }
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("confidence")]
        public Level Confidence { get; set; }
        [JsonProperty("importance")]
        public Level Importance { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("recommendation_data")]
        public object RecommendationData { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
