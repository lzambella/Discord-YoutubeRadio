using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookSharp.GraphAPI
{
    public class Data
    {
        /// <summary>
        /// The achievement type that the user achieved.
        /// </summary>
        AchievementType achievement { get; set; }
        /// <summary>
        /// A weighting given to each achievement type by the app
        /// </summary>
        int importance { get; set; }
    }
}
