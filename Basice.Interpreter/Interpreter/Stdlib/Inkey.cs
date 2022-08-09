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
            if (interpreter.TextOutputDevice.GetScreen() == 1)
            {
                return interpreter.TextInputDevice.GetNextChar().ToString();
            }

            return interpreter.GraphicsInputDevice.GetNextChar().ToString();
        }
    }
}
