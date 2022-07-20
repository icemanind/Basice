using System;
using System.Collections.Generic;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Year : ICallable
    {
        public int NumberOfArguments()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            return (double)DateTime.Now.Year;
        }
    }
}
