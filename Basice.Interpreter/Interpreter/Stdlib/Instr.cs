using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;
using System.Collections.Generic;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Instr : ICallable
    {
        public int NumberOfArguments()
        {
            return 3;
        }

        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            if (!(arguments[0] is double))
            {
                throw new RuntimeException("INSTR starting position argument must be a number.", token);
            }

            if (!(arguments[1] is string))
            {
                throw new RuntimeException("INSTR source string argument must be a string.", token);
            }

            if (!(arguments[2] is string))
            {
                throw new RuntimeException("INSTR target string argument must be a string.", token);
            }

            string source = (string)arguments[1];
            string dest = (string)arguments[2];
            int start = (int)(double)arguments[0];

            int location = source.IndexOf(dest, start - 1);

            return (double)(location + 1);
        }
    }
}
