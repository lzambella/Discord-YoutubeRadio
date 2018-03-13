using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    public class Data
    {
        /// <summary>
        /// The achievement type that the user achieved.
        /// </summary>
        [JsonProperty("achievement")]
        public AchievementType Achievement { get; set; }
        /// <summary>
        /// A weighting given to each achievement type by the app
        /// </summary>
        [JsonProperty("importance")]
        public int Importance { get; set; }
    }
}
