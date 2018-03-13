using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace FacebookSharp.GraphAPI
{
    public class AchievementImage
    {
        /// <summary>
        /// Location of the achievement image
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// Pixel width of the image
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }
        /// <summary>
        /// Pixel height of the image
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }
    }
}
