using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// A place
    /// </summary>
    public class Place
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("location")]
        public object Location { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("overall_rating")]
        public float OverallRating { get; set; }
    }
}
