using System.Collections.Generic;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Interpreter
{
    public interface ICallable
    {
        int NumberOfArguments();
        object Call(Interpreter interpreter, Token token, List<object> arguments);
    }
}
