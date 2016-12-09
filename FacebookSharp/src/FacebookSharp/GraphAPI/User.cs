using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookSharp.GraphAPI
{
    public class User
    {
        /// <summary>
        /// The id of this person's user account. This ID is unique to each app and cannot be used across different apps. 
        /// </summary>
        string id { get; set; }
        /// <summary>
        /// Equivalent to the bio field
        /// </summary>
        string about { get; set; }

    }
}
