using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// Object that contains array of type Photo and Paging
    /// </summary>
    public class PagePhotos
    {
        [JsonProperty("data")]
        public IList<Photo> PhotoNodes { get; set; }
        [JsonProperty("paging")]
        public object Paging { get; set; }
    }
}
