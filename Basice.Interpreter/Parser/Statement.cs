﻿using System.Collections.Generic;
using Basice.Interpreter.Lexer;

namespace Basice.Interpreter.Parser
{
    public abstract class Statement
    {
        public int BasicLineNumber { get; set; }

        public class ArcStatement : Statement
        {
            public Expression X { get; }
            public Expression Y { get; }
            public Expression Width { get; }
            public Expression Height { get; }
            public Expression Color { get; }
            public Expression Start { get; }
            public Expression End { get; }

            public ArcStatement(Expression x, Expression y, Expression width, Expression height, Expression color, Expression start, Expression end)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Color = color;
                Start = start;
                End = end;
            }
        }

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

        public class ColorStatement : Statement
        {
            public Expression ForegroundColor { get; }
            public Expression BackgroundColor { get; }

            public ColorStatement(Expression foregroundColor, Expression backgroundColor, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                ForegroundColor = foregroundColor;
                BackgroundColor = backgroundColor;
            }
        }

        public class CursorStatement : Statement
        {
            public bool CursorOn { get; set; }

            public CursorStatement(bool cursorOn, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                CursorOn = cursorOn;
            }
        }

        public class DataStatement : Statement
        {
            public Queue<Expression> Data { get; }

            public DataStatement(Queue<Expression> data, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                Data = data;
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

        public class DrawTextStatement : Statement
        {
            public Expression X { get; }
            public Expression Y { get; }
            public Expression Size { get; }
            public Expression Text { get; }
            public Expression Color { get; }

            public DrawTextStatement(Expression x, Expression y, Expression text, Expression size, Expression color)
            {
                Text = text;
                Size = size;
                X = x;
                Y = y;
                Color = color;
            }
        }

        public class EllipseStatement : Statement
        {
            public Expression X { get; }
            public Expression Y { get; }
            public Expression Width { get; }
            public Expression Height { get; }
            public Expression Color { get; }

            public EllipseStatement(Expression x, Expression y, Expression width, Expression height, Expression color)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Color = color;
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

        public class GosubStatement : Statement
        {
            public int LineNumber { get; }

            public GosubStatement(int lineNumber, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                LineNumber = lineNumber;
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

        public class InputStatement : Statement
        {
            public Token Name { get; }
            public bool IsArray { get; }
            public List<Expression> ArrayIndexes { get; }

            public InputStatement(Token name, bool isArray, List<Expression> arrayIndexes, int basicLineNumber)
            {
                Name = name;
                BasicLineNumber = basicLineNumber;
                IsArray = isArray;
                ArrayIndexes = arrayIndexes;
            }
        }

        public class LineStatement : Statement
        {
            public Expression X1 { get; }
            public Expression X2 { get; }
            public Expression Y1 { get; }
            public Expression Y2 { get; }
            public Expression Color { get; }

            public LineStatement(Expression x1, Expression y1, Expression x2, Expression y2, Expression color,
                int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                X1 = x1;
                X2 = x2;
                Y1 = y1;
                Y2 = y2;
                Color = color;
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

        public class PointStatement : Statement
        {
            public Expression X { get; }
            public Expression Y { get; }
            public Expression Color { get; set; }

            public PointStatement(Expression x, Expression y, Expression color, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                X = x;
                Y = y;
                Color = color;
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

        public class ReadStatement : Statement
        {
            public Token Name { get; }
            public bool IsArray { get; }
            public List<Expression> ArrayIndexes { get; }

            public ReadStatement(Token name, bool isArray, List<Expression> arrayIndexes, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                IsArray = isArray;
                ArrayIndexes = arrayIndexes;
                Name = name;
            }
        }

        public class ReturnStatement : Statement
        {
            public ReturnStatement(int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
            }
        }

        public class RestoreStatement : Statement
        {
            public RestoreStatement(int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
            }
        }

        public class ScreenStatement : Statement
        {
            public Expression Number { get; }

            public ScreenStatement(Expression number, int basicLineNumber)
            {
                BasicLineNumber = basicLineNumber;
                Number = number;
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
