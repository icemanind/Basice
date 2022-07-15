﻿using System.Collections.Generic;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Parser
{
    public abstract class Statement
    {
        public int BasicLineNumber { get; set; }

        public class Block : Statement
        {
            public List<Statement> Statements { get; }

            public Block(List<Statement> statements, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                Statements = statements;
            }
        }

        public class ClsStatement : Statement
        {
            public ClsStatement(int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
            }
        }

        public class DimStatement : Statement
        {
            public Token Name { get; }
            public List<Expression> Capacities { get; }

            public DimStatement(Token name, List<Expression> capacities, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                Name = name;
                Capacities = capacities;
            }
        }

        public class EndStatement : Statement
        {
            public EndStatement(int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
            }
        }

        public class ForStatement : Statement
        {
            public Token Variable { get; }
            public Expression Start { get; }
            public Expression End { get; }
            public Expression Step { get; }

            public ForStatement(Token variable, Expression start, Expression end, Expression step, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                Variable = variable;
                Start = start;  
                End = end;
                Step = step;
            }
        }

        public class GotoStatement : Statement
        {
            public int LineNumber { get; }

            public GotoStatement(int lineNumber, int basicLineNumber)
            {
                BasicLineNumber= basicLineNumber;
                LineNumber = lineNumber;
            }
        }

        public class IfStatement : Statement
        {
            public Expression Condition { get; }
            public Statement ThenBranch { get; }
            public Statement ElseBranch { get; }

            public IfStatement(Expression condition, Statement thenBranch, Statement elseBranch, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }
        }

        public class IncrementStatement : Statement
        {
            public Token Token { get; }
            public Expression Value { get; }

            public IncrementStatement(Token token, Expression value, int basicLineNumber)
            {
                BasicLineNumber=basicLineNumber;
                Token = token;
                Value = value;
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

        public class NextStatement : Statement
        {
            public NextStatement(int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
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
            public bool AddCrLf { get; }

            public PrintStatement(Expression expression, bool addCrLf, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                Expression = expression;
                AddCrLf = addCrLf;
            }
        }

        public class VariableArrayStatement : Statement
        {
            public Token Name { get; }
            public Expression Initializer { get; }
            public List<Expression> Indices { get; }

            public VariableArrayStatement(Token name, List<Expression> indices, Expression initializer, int basicLineNumber)
            {
                Name = name;
                Indices = indices;
                Initializer = initializer;
                BasicLineNumber = basicLineNumber;
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
