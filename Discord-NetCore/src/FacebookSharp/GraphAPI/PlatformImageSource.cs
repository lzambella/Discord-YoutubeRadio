using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// Image source and dimensions
    /// </summary>
    public class PlatformImageSource
    {
        /// <summary>
        /// Height of the image in pixels
        /// </summary>
        [JsonProperty("height")]
        public uint Height { get; set; }
        /// <summary>
        /// Source URL of the image
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }
        /// <summary>
        /// Width of the image in pixels
        /// </summary>
        [JsonProperty("width")]
        public uint Width { get; set; }
    }
}
