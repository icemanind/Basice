using System;
using System.Collections.Generic;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Rnd : ICallable
    {
        private readonly Random _random;

        public Rnd()
        {
            _random = new Random();
        }

        public int NumberOfArguments()
        {
            return 1;
        }

        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            if (!(arguments[0] is double))
            {
                throw new RuntimeException("RND argument must be a number.", token);
            }

            int intValue = (int)(double)arguments[0];
            int rndValue = _random.Next(0, intValue + 1);

            return (double)rndValue;
        }
    }
}
