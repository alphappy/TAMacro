using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace alphappy.TAMacro
{
    public class Exceptions
    {
        public class TAMacroException : Exception
        {
            public TAMacroException() { }
            public TAMacroException(string message) : base(message) { }
            public TAMacroException(string message, Exception innerException) : base(message, innerException) { }
            protected TAMacroException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
        public class IllegalCommandException : TAMacroException
        {
            public IllegalCommandException() { }
            public IllegalCommandException(string message) : base(message) { }
            public IllegalCommandException(string message, Exception innerException) : base(message, innerException) { }
            protected IllegalCommandException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
    }
}
