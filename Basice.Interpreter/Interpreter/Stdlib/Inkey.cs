using System.Collections.Generic;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Inkey : ICallable
    {
        public int NumberOfArguments()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            char c = interpreter.TextInputDevice.GetNextChar();

            return c.ToString();
        }
    }
}
