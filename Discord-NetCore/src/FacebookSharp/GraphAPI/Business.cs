using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class Business
    {
        /// <summary>
        /// The name of the business
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// ID for the timezone
        /// </summary>
        [JsonProperty("timezone_id")]
        public int TimezoneId { get; set; }
        /// <summary>
        /// The object of the primary page associated with this business manager.
        /// </summary>
        [JsonProperty("primary_usage")]
        public PrimaryPage PrimaryUsage { get; set; }
        /// <summary>
        /// The ID of the business manager
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }
        /// <summary>
        /// The last time this business manager was updated
        /// </summary>
        [JsonProperty("update_time")]
        public string UpdateTime { get; set; }
        /// <summary>
        /// The last user(name and id) who have updated this business manager
        /// </summary>
        [JsonProperty("updated_by")]
        public BasicUser UpdatedBy { get; set; }
        /// <summary>
        /// The time this brand was created
        /// </summary>
        [JsonProperty("creation_time")]
        public string CreationTime { get; set; }
        /// <summary>
        /// The user (name and id) who has created this business manager
        /// </summary>
        [JsonProperty("created_by")]
        public BasicUser CreatedBy { get; set; }
    }

    public class BasicUser
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
