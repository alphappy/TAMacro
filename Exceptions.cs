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
            public string MultiLineMessage => Message.Replace("  ", "\n");
        }
        public class MacroRuntimeException : TAMacroException
        {
            public MacroRuntimeException(Macro macro, string message) : base($"An exception occurred while running a macro.\n  Macro: {macro.FullName}\n  Line number: {macro.currentLine}\n  Line: {macro.currentLineText}\n  Instruction number: {macro.currentIndex}\n  Instruction: {macro.current.type}\n  Operand: {macro.current.value}\n{message}") { }

            public MacroRuntimeException(string message) : base(message) { }
        }
        public class IllegalCommandException : TAMacroException
        {
            public IllegalCommandException() { }
        }
        public class InvalidExecuteTargetException : MacroRuntimeException
        {
            public InvalidExecuteTargetException(Macro macro, string message) : base(macro, $"`>execute` command failed:  \n{message}") { }
            public InvalidExecuteTargetException(string message) : base($"`>execute` command failed:  \n{message}") { }
        }
    }
}
