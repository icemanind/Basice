using System.Collections.Generic;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Mid : ICallable
    {
        public int NumberOfArguments()
        {
            return 3;
        }

        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            if (!(arguments[0] is string))
            {
                throw new RuntimeException("MID$ argument 1 must be a string.", token);
            }
            if (!(arguments[1] is double))
            {
                throw new RuntimeException("MID$ argument 2 must be a number.", token);
            }
            if (!(arguments[2] is double))
            {
                throw new RuntimeException("MID$ argument 3 must be a number.", token);
            }

            string value = (string)arguments[0];
            int position = (int)(double)arguments[1];
            int length = (int)(double)arguments[2];

            if (length <= 0) return "";
            
            try
            {
                return value.Substring(position - 1, length);
            }
            catch
            {
                return "";
            }
        }
    }
}
