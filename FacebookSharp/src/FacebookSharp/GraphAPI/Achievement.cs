using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookSharp.GraphAPI
{
    public class Achievement
    {
        /// <summary>
        /// ID of this particular achievement.
        /// </summary>
        string id { get; set; }
        /// <summary>
        /// The user who achieved this.
        /// </summary>
        User from { get; set; }
        /// <summary>
        /// Time at which this was achieved.
        /// </summary>
        DateTime publish_time { get; set; }
        /// <summary>
        /// The app in which the user achieved this.
        /// </summary>
        App application { get; set; }
        /// <summary>
        /// Information about the achievement type this instance is connected with.
        /// </summary>
        Data data { get; set; }
        /// <summary>
        /// Always game.achievement.
        /// </summary>
        string type { get; set; }
        /// <summary>
        /// Indicates whether gaining the achievement published a feed story for the user.
        /// </summary>
        bool no_feed_story { get; set; }
    }
}
