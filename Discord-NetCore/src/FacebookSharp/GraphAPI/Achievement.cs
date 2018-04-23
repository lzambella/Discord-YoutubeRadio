using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    public class Achievement
    {
        /// <summary>
        /// ID of this particular achievement.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// The user who achieved this.
        /// </summary>
        [JsonProperty("from")]
        public User From { get; set; }
        /// <summary>
        /// Time at which this was achieved.
        /// </summary>
        [JsonProperty("publish_time")]
        public DateTime PublishTime { get; set; }
        /// <summary>
        /// The app in which the user achieved this.
        /// </summary>
        [JsonProperty("application")]
        public App Application { get; set; }
        /// <summary>
        /// Information about the achievement type this instance is connected with.
        /// </summary>
        [JsonProperty("data")]
        public Data Data { get; set; }
        /// <summary>
        /// Always game.achievement.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// Indicates whether gaining the achievement published a feed story for the user.
        /// </summary>
        [JsonProperty("no_feed_story")]
        public bool NoFeedStory { get; set; }
    }
}
