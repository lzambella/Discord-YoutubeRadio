using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreBot.Modules.facebook
{
    /// <summary>
    /// Deserialization class that contains the photo's resolution and direct link
    /// For facebook graph api 2.5
    /// </summary>
    public class PhotoData
    {
        /// <summary>
        /// Height in pixels
        /// </summary>
        public string height { get; set; }
        /// <summary>
        /// Source URL of the image
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// Widht in pixels
        /// </summary>
        public string width { get; set; }
    }
}
