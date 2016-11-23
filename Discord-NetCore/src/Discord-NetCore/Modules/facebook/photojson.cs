using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreBot.Modules.facebook
{

    public class photojson
    {
        public IList<ImageArray> data;
        public cursor cursors { get; set; }
        public string next { get; set; }
    }
}
