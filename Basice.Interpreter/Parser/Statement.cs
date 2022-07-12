using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Parser
{
    public abstract class Statement
    {
        public int BasicLineNumber { get; set; }

        public class ClsStatement : Statement
        {
            public ClsStatement(int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
            }
        }

        public class LocateStatement : Statement
        {
            public Expression Y { get; }
            public Expression X { get; }

            public LocateStatement(Expression y, Expression x, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                Y = y;
                X = x;
            }
        }

        public class NopStatement : Statement
        {
            public NopStatement(int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
            }
        }

        public class PrintStatement : Statement
        {
            public Expression Expression { get; }

            public PrintStatement(Expression expression, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                Expression = expression;
            }
        }

        public class VariableStatement : Statement
        {
            public Token Name { get; }
            public Expression Initializer { get; }

            public VariableStatement(Token name, Expression initializer, int basicLineNumber)
            {
                Name = name;
                Initializer = initializer;
                BasicLineNumber = basicLineNumber;
            }
        }
    }
}
