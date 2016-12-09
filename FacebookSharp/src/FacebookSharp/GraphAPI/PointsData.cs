using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// Container object for containing AchievementType.data
    /// </summary>
    public class PointsData
    {
        /// <summary>
        /// Number of points that this achievement is worth. Total points per game may not exceed 1000 points limit, which is enforced
        /// </summary>
        int points { get; set; }
    }
}
