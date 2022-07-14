using System.Collections.Generic;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Parser
{
    public abstract class Expression
    {
        public class ArrayVariable : Expression
        {
            public Token Name { get; }
            public Expression Capacity { get; }

            public ArrayVariable(Token name, Expression capacity)
            {
                Name = name;
                Capacity = capacity;
            }
        }

        public class Binary : Expression
        {
            public Expression Left { get; }
            public Expression Right { get; }
            public Token Operator { get; }

            public Binary(Expression left, Token @operator, Expression right)
            {
                Left = left;
                Right = right;
                Operator = @operator;
            }
        }

        public class Call : Expression
        {
            public Expression Callee { get; }
            public Token Paren { get; }
            public List<Expression> Arguments { get; }

            public Call(Expression callee, Token paren, List<Expression> arguments)
            {
                Callee = callee;
                Paren = paren;
                Arguments = arguments;
            }
        }

        public class Grouping : Expression
        {
            public Expression Expression { get; }

            public Grouping(Expression expression)
            {
                Expression = expression;
            }
        }

        public class Literal : Expression
        {
            public object Value { get; }

            public Literal(object value)
            {
                Value = value;
            }
        }

        public class Logical : Expression
        {
            public Expression Left { get; }
            public Token Operator { get; }
            public Expression Right { get; }

            public Logical(Expression left, Token @operator, Expression right)
            {
                Left = left;
                Operator = @operator;
                Right = right;
            }
        }

        public class Unary : Expression
        {
            public Token Operator { get; }
            public Expression Right { get; }

            public Unary(Token @operator, Expression right)
            {
                Operator = @operator;
                Right = right;
            }
        }

        public class Variable : Expression
        {
            public Token Name { get; }

            public Variable(Token name)
            {
                Name = name;
            }
        }
    }
}
