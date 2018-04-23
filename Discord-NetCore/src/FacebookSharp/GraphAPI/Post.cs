using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// A post
    /// </summary>
    public class Post
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("admin_creator")]
        public AdminCreator AdminCreator { get; set; }

        [JsonProperty("application")]
        public App Application { get; set; }

        [JsonProperty("call_toAction")]
        public object CallToAction { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }
        
        //TODO:
    }

    public class AdminCreator
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
