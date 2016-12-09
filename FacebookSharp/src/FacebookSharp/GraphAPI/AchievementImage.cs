using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookSharp.GraphAPI
{
    public class AchievementImage
    {
        /// <summary>
        /// Location of the achievement image
        /// </summary>
        string url { get; set; }
        /// <summary>
        /// Pixel width of the image
        /// </summary>
        int width { get; set; }
        /// <summary>
        /// Pixel height of the image
        /// </summary>
        int height { get; set; }
    }
}
