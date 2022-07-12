using System;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Exceptions
{
    public class RuntimeException : Exception
    {
        public Token Token { get; }

        public RuntimeException()
        {
        }

        public RuntimeException(string message)
            : base(message)
        {
        }

        public RuntimeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public RuntimeException(string message, Token token) : base($"Line [{token.Line}]: {message}")
        {
            Token = token;
        }

        public RuntimeException(string message, int lineNumber) : base($"Line [{lineNumber}]: {message}")
        {
        }
    }
}
