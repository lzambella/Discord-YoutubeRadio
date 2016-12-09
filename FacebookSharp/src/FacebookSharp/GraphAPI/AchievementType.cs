using System;
using System.Collections.Generic;

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
        string id { get; set; }
        /// <summary>
        /// The value will be games.achievement
        /// </summary>
        string type { get; set; }
        /// <summary>
        /// Title of achievement
        /// </summary>
        string title { get; set; }
        /// <summary>
        /// Unique URL of the achievement
        /// </summary>
        string url { get; set; }
        /// <summary>
        /// Description of the achievement
        /// </summary>
        string description { get; set; }
        /// <summary>
        /// Image for the achievement
        /// </summary>
        AchievementImage image { get; set; }
        /// <summary>
        /// An object containing the points this achievement is worth.
        /// </summary>
        PointsData data { get; set; }
        /// <summary>
        /// Time when the achievement was last updated
        /// </summary>
        DateTime updated_time { get; set; }
        /// <summary>
        /// Time when the achievement was created
        /// </summary>
        DateTime created_time { get; set; }
        /// <summary>
        /// The app that created the achievement.
        /// </summary>
        App application { get; set; }
        /// <summary>
        /// Context of the achievement for the associated app
        /// </summary>
        IList<int> context { get; set; }
        /// <summary>
        /// Whether the URL containing the achievement metadata has been scraped by Facebook servers.
        /// </summary>
        bool is_scraped { get; set; }
    }
}
