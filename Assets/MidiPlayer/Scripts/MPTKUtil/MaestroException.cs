using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiPlayerTK
{
    public class MaestroException : ApplicationException
    {
        public MaestroException(string Message, Exception innerException) : base(Message, innerException) { }
        public MaestroException(string Message) : base(Message) { }
        public MaestroException(int Code) { }
        public MaestroException() { }
    }
}
