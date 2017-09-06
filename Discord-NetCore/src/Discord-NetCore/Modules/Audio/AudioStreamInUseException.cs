using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_NetCore.Modules.Audio
{
    public class AudioStreamInUseException : Exception
    {
        public AudioStreamInUseException()
        {

        }
        public AudioStreamInUseException(string message):base(message)
        {
        }
    }
}
