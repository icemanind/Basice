using System.Collections.Generic;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Asc : ICallable
    {
        public int NumberOfArguments()
        {
            return 1;
        }

        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            if (!(arguments[0] is string))
            {
                throw new RuntimeException("ASC argument must be a string.", token);
            }

            string strValue = (string)arguments[0];

            if (strValue.Length == 0) return (double)0;

            return (double)((int)strValue[0]);
        }
    }
}
