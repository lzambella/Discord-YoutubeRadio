using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    public class Photo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("album")]
        public Album PhotoAlbum { get; set; }
        [JsonProperty("backdated_time")]
        public string BackdatedTime { get; set; }
        [JsonProperty("backdated_time_granularity")]
        public int BackdatedTimeGranularity { get; set; }
        [JsonProperty("can_backdate")]
        public bool CanBackdate { get; set; }
        [JsonProperty("can_delete")]
        public bool CanDelete { get; set; }
        [JsonProperty("can_tag")]
        public bool CanTag { get; set; }
        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }
        [JsonProperty("event")]
        public Event AssociatedEvent { get; set; }
        [JsonProperty("from")]
        public User From { get; set; }
        [JsonProperty("height")]
        public UInt32 Height { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("images")]
        public IList<PlatformImageSource> Images { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("name_tags")]
        public IList<EntityAtTextRange> NameTags { get; set; }
        [JsonProperty("page_story_id")]
        public string PageStoryId { get; set; }
        [JsonProperty("picture")]
        public string Picture { get; set; }
        [JsonProperty("place")]
        public object Place { get; set; }
        [JsonProperty("position")]
        public UInt32 Position { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("target")]
        public object Target { get; set; }
        [JsonProperty("updated_time")]
        public string UpdatedTime { get; set; }
        [JsonProperty("webp_images")]
        public IList<object> WebpImages { get; set; }
        [JsonProperty("width")]
        public UInt32 Width { get; set; }

    }
}
