using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreBot.Modules.facebook
{
    /// <summary>
    /// Deserialization class that contains an IList of image data.
    /// </summary>
    public class ImageArray
    {
        /// <summary>
        /// image array json object
        /// </summary>
        public IList<PhotoData> images { get; set; }
    }
}
