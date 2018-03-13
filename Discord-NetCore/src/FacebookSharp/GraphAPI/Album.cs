using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// Represents a photo album
    /// </summary>
    public class Album
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("can_upload")]
        public bool CanUpload { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("cover_photo")]
        public PagePhotos CoverPhoto { get; set; }
        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("event")]
        public Event PhotoEvent { get; set; }
        [JsonProperty("from")]
        public User From { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("place")]
        public Page Place { get; set; }
        [JsonProperty("privacy")]
        public string Privacy { get; set; }
        // Todo: convert to enum
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("updated_time")]
        public string UpdatedTime { get; set; }
    }
}
