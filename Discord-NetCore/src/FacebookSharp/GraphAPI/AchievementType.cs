using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// A games achievement type created by a Facebook App. Not to be confused with an instance of an achievement.
    /// </summary>
    public class AchievementType
    {
        /// <summary>
        /// ID of the achievement type
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// The value will be games.achievement
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// Title of achievement
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// Unique URL of the achievement
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// Description of the achievement
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// Image for the achievement
        /// </summary>
        [JsonProperty("image")]
        public AchievementImage Image { get; set; }
        /// <summary>
        /// An object containing the points this achievement is worth.
        /// </summary>
        [JsonProperty("data")]
        public PointsData Data { get; set; }
        /// <summary>
        /// Time when the achievement was last updated
        /// </summary>
        //TODO: Change to DateTime later
        [JsonProperty("updated_time")]
        public string UpdatedTime { get; set; }
        /// <summary>
        /// Time when the achievement was created
        /// </summary>
        //TODO: Change to DateTime later
        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }
        /// <summary>
        /// The app that created the achievement.
        /// </summary>
        [JsonProperty("application")]
        public App Application { get; set; }
        /// <summary>
        /// Context of the achievement for the associated app
        /// </summary>
        [JsonProperty("context")]
        public IList<int> Context { get; set; }
        /// <summary>
        /// Whether the URL containing the achievement metadata has been scraped by Facebook servers.
        /// </summary>
        [JsonProperty("is_scraped")]
        public bool IsScraped { get; set; }
    }
}
