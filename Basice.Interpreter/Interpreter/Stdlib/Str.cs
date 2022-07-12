using System.Collections.Generic;
using System.Globalization;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Str : ICallable
    {
        public int NumberOfArguments()
        {
            return 1;
        }

        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            if (!(arguments[0] is double))
            {
                throw new RuntimeException("STR$ argument must be a number.", token);
            }

            double value = (double)arguments[0];
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
