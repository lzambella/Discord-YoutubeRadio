using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// Container for primary_page in Business.cs
    /// </summary>
    public class PrimaryPage
    {
        [JsonProperty("catagory")]
        public string Catagory { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
