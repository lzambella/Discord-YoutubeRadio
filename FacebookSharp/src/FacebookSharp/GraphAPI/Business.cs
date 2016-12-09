using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        string name { get; set; }
        /// <summary>
        /// ID for the timezone
        /// </summary>
        int timezone_id { get; set; }
        /// <summary>
        /// The object of the primary page associated with this business manager.
        /// </summary>
        PrimaryPage primary_page { get; set; }
        /// <summary>
        /// The ID of the business manager
        /// </summary>
        long id { get; set; }
        /// <summary>
        /// The last time this business manager was updated
        /// </summary>
        string update_time { get; set; }
        /// <summary>
        /// The last user(name and id) who have updated this business manager
        /// </summary>
        BasicUser updated_by { get; set; }
        /// <summary>
        /// The time this brand was created
        /// </summary>
        string creation_time { get; set; }
        /// <summary>
        /// The user (name and id) who has created this business manager
        /// </summary>
        BasicUser created_by { get; set; }
    }

    internal class BasicUser
    {
        string name { get; set; }
        string id { get; set; }
    }
}
