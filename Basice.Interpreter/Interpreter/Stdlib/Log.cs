using System;
using System.Collections.Generic;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Log : ICallable
    {
        public int NumberOfArguments()
        {
            return 1;
        }

        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            if (!(arguments[0] is double))
            {
                throw new RuntimeException("LOG argument must be a number.", token);
            }

            double dblValue = (double)arguments[0];

            return Math.Log(dblValue);
        }
    }
}
