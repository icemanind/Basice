using System.Collections.Generic;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Right : ICallable
    {
        public int NumberOfArguments()
        {
            return 2;
        }

        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            if (!(arguments[0] is string))
            {
                throw new RuntimeException("RIGHT$ argument 1 must be a string.", token);
            }
            if (!(arguments[1] is double))
            {
                throw new RuntimeException("RIGHT$ argument 2 must be a number.", token);
            }

            string value = (string)arguments[0];
            int length = (int)(double)arguments[1];

            if (length <= 0) return "";
            if (length > value.Length) return value;

            return value.Substring(value.Length - length);
        }
    }
}
