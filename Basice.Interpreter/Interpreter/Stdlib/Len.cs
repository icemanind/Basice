using System.Collections.Generic;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Len : ICallable
    {
        public int NumberOfArguments()
        {
            return 1;
        }

        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            if (!(arguments[0] is string))
            {
                throw new RuntimeException("Len argument must be a string.", token);
            }

            string strValue = (string)arguments[0];

            return (double)strValue.Length;
        }
    }
}
