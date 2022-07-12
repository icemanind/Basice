using System.Collections.Generic;
using System.Globalization;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter.Stdlib
{
    public class Rgb : ICallable
    {
        public object Call(Interpreter interpreter, Token token, List<object> arguments)
        {
            if (!(arguments[0] is double))
            {
                throw new RuntimeException("Argument 'red' must be a number.", token);
            }
            if (!(arguments[1] is double))
            {
                throw new RuntimeException("Argument 'green' must be a number.", token);
            }
            if (!(arguments[2] is double))
            {
                throw new RuntimeException("Argument 'blue' must be a number.", token);
            }

            int red = (int)(double)arguments[0];
            int green = (int)(double)arguments[1];
            int blue = (int)(double)arguments[2];

            if (red < 0 || red > 255)
            {
                throw new RuntimeException("Argument 'red' must be a number between 0 and 255.", token);
            }
            if (green < 0 || green > 255)
            {
                throw new RuntimeException("Argument 'green' must be a number between 0 and 255.", token);
            }
            if (blue < 0 || blue > 255)
            {
                throw new RuntimeException("Argument 'blue' must be a number between 0 and 255.", token);
            }
            string hex = $"{red:x2}{green:x2}{blue:x2}";

            return (double)int.Parse(hex, NumberStyles.HexNumber);
        }

        public int NumberOfArguments()
        {
            return 3;
        }
    }
}
