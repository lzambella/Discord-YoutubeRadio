using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreBot.Modules
{
    /// <summary>
    /// Class for easilly getting data from a database
    /// </summary>
    public class RPGObject
    {
        /// <summary>
        /// X Coord of the object
        /// </summary>
        public int XPos { get; set; }
        /// <summary>
        /// Y Coord of the object
        /// </summary>
        public int YPos { get; set; }
        /// <summary>
        /// Object ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Create a new object 
        /// </summary>
        public RPGObject(int x, int y, int id)
        {
            XPos = x;
            YPos = y;
            ID = id;
        }
    }
}
