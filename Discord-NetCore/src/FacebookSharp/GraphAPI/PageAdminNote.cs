using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// Page's admin note
    /// </summary>
    public class PageAdminNote
    {
        /// <summary>
        /// Content of this note
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }
        
    }
}
