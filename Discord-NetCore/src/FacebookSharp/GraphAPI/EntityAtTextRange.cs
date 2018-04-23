using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// Location and identity of an object in some source text
    /// </summary>
    public class EntityAtTextRange
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("length")]
        public uint Length { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("object")]
        public IProfile ProfileObject { get; set; }
        [JsonProperty("offset")]
        public uint Offset { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
    }
}
