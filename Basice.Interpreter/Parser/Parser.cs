using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;
using System.Collections.Generic;

namespace Basice.Interpreter.Parser
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private readonly List<int> _basicLineNumbers;
        private int _current;
        private int _currentBasicLineNumber;
        private int _previousBasicLineNumber;
        private readonly Stack<Statement.ForStatement> _forStack;

        public Parser(List<Token> tokens)
        {
            _forStack = new Stack<Statement.ForStatement>();
            _basicLineNumbers = new List<int>();
            _tokens = tokens;
            _current = 0;
            _currentBasicLineNumber = -1;
            _previousBasicLineNumber = -1;
        }

        public List<Statement> Parse()
        {
            _forStack.Clear();
            var statements = new List<Statement>();

            while (!IsAtEnd())
            {
                statements.Add(LineNumber());

                if (Peek().Type == TokenType.NewLine)
                {
                    while (!IsAtEnd() && Match(TokenType.NewLine)) ;
                }
                else if (Peek().Type == TokenType.Colon)
                {
                    while (!IsAtEnd() && Match(TokenType.Colon))
                    {
                        statements.Add(Declaration());
                    }
                }
                else
                {
                    throw new ParserException(Error("Expected carriage return or ':' after statement.", Previous()));
                }

                if (Peek().Type == TokenType.NewLine)
                {
                    while (!IsAtEnd() && Match(TokenType.NewLine)) ;
                }
            }

            if (_forStack.Count > 0)
            {
                throw new ParserException(Error("Missing 'NEXT' statement.", Previous()));
            }

            return statements;
        }

        private Statement LineNumber()
        {
            if (!Match(TokenType.Number))
            {
                throw new ParserException(Error("Expected line number.", Previous()));
            }

            string strNumber = Previous().Literal.ToString();
            if (strNumber.Contains("."))
            {
                throw new ParserException(Error("Line numbers must be integers.", Previous()));
            }

            _previousBasicLineNumber = _currentBasicLineNumber;

            _currentBasicLineNumber = int.Parse(strNumber);
            if (_currentBasicLineNumber < 0)
            {
                throw new ParserException(Error("Line numbers must not be negative numbers.", Previous()));
            }

            if (_previousBasicLineNumber != -1 && _currentBasicLineNumber < _previousBasicLineNumber)
            {
                throw new ParserException(Error("Line numbers should increase in order.", Previous()));
            }

            if (_basicLineNumbers.Contains(_currentBasicLineNumber))
            {
                throw new ParserException(Error("Duplicate line number.", Previous()));
            }
            _basicLineNumbers.Add(_currentBasicLineNumber);

            return NoOperation();
        }

        private string Error(string message, Token token)
        {
            return $"Line [{token.Line}]: {message}";
        }

        private Statement NoOperation()
        {
            if (Match(TokenType.Nop)) return new Statement.NopStatement(_currentBasicLineNumber);

            return Declaration();
        }

        private Statement Declaration()
        {
            if (Peek().Type == TokenType.Identifier && PeekNext().Type == TokenType.LeftParenthesis)
            {
                var indices = new List<Expression>();
                Token token = Peek();
                Advance();
                Advance();

                do
                {
                    Expression indexExpression = Expression();
                    indices.Add(indexExpression);
                    if (Peek().Type != TokenType.Comma) break;
                    Advance();
                } while (true);

                if (!Match(TokenType.RightParenthesis))
                {
                    throw new ParserException(Error("Expected ')' after array index", Peek()));
                }

                if (Previous().Type == TokenType.Comma)
                {

                }

                if (Peek().Type == TokenType.Equal)
                {
                    Advance();
                    Expression expression = Expression();

                    return new Statement.VariableArrayStatement(token, indices, expression,
                        _currentBasicLineNumber);
                }

                return Statement();
            }

            if (Match(TokenType.Identifier) && Peek().Type == TokenType.Equal)
            {
                Token token = Previous();
                Advance();
                Expression expression = Expression();

                return new Statement.VariableStatement(token, expression, _currentBasicLineNumber);
            }

            return Statement();
        }

        private Statement Statement()
        {
            if (Match(TokenType.Cls)) return ClsStatement();
            if (Match(TokenType.Cursor)) return CursorStatement();
            if (Match(TokenType.Data)) return DataStatement();
            if (Match(TokenType.Dim)) return DimStatement();
            if (Match(TokenType.End)) return EndStatement();
            if (Match(TokenType.For)) return ForStatement();
            if (Match(TokenType.Gosub)) return GosubStatement();
            if (Match(TokenType.Goto)) return GotoStatement();
            if (Match(TokenType.If)) return IfStatement();
            if (Match(TokenType.Locate)) return LocateStatement();
            if (Match(TokenType.Next)) return NextStatement();
            if (Match(TokenType.Print)) return PrintStatement();
            if (Match(TokenType.Read)) return ReadStatement();
            if (Match(TokenType.Restore)) return RestoreStatement();
            if (Match(TokenType.Return)) return ReturnStatement();

            throw new ParserException(Error($"Unrecognized statement '{_tokens[_current].Lexeme}'", Previous()));
        }

        #region "Expressions"
        private Expression Expression()
        {
            return Or();
        }

        private Expression Or()
        {
            Expression expression = And();

            while (Match(TokenType.Or))
            {
                Token op = Previous();
                Expression right = And();
                expression = new Expression.Logical(expression, op, right);
            }

            return expression;
        }

        private Expression And()
        {
            Expression expression = Equality();

            while (Match(TokenType.And))
            {
                Token op = Previous();
                Expression right = And();
                expression = new Expression.Logical(expression, op, right);
            }

            return expression;
        }

        private Expression Equality()
        {
            Expression expression = Comparison();

            while (Match(TokenType.NotEqual, TokenType.Equal))
            {
                Token op = Previous();
                Expression right = Comparison();
                expression = new Expression.Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Comparison()
        {
            Expression expression = Term();

            while (Match(TokenType.GreaterThan, TokenType.GreaterThanOrEqual, TokenType.LessThan,
                       TokenType.LessThanOrEqual))
            {
                Token op = Previous();
                Expression right = Term();
                expression = new Expression.Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Term()
        {
            Expression expression = Factor();

            while (Match(TokenType.Minus, TokenType.Plus))
            {
                Token @operator = Previous();
                Expression right = Factor();
                expression = new Expression.Binary(expression, @operator, right);
            }

            return expression;
        }

        private Expression Factor()
        {
            Expression expression = Unary();

            while (Match(TokenType.Star, TokenType.Slash))
            {
                Token @operator = Previous();
                Expression right = Unary();
                expression = new Expression.Binary(expression, @operator, right);
            }

            return expression;
        }

        private Expression Unary()
        {
            if (Match(TokenType.Minus) || Match(TokenType.Not))
            {
                Token @operator = Previous();
                Expression right = Unary();
                return new Expression.Unary(@operator, right);
            }

            return Call();
        }

        private Expression Call()
        {
            Expression expression = Primary();

            while (true)
            {
                if (Match(TokenType.LeftParenthesis))
                {
                    expression = FinishCall(expression);
                }
                else
                {
                    break;
                }
            }

            return expression;
        }

        private Expression FinishCall(Expression callee)
        {
            var arguments = new List<Expression>();

            if (!Check(TokenType.RightParenthesis))
            {
                do
                {
                    arguments.Add(Expression());
                } while (Match(TokenType.Comma));
            }

            if (Check(TokenType.RightParenthesis))
            {
                Token paren = Advance();
                return new Expression.Call(callee, paren, arguments);
            }

            throw new ParserException(Error("Expect ')' after arguments.", Peek()));
        }

        private Expression Primary()
        {
            if (Match(TokenType.Number, TokenType.String))
            {
                return new Expression.Literal(Previous().Literal);
            }

            if (Match(TokenType.Identifier))
            {
                return new Expression.Variable(Previous());
            }

            if (Match(TokenType.LeftParenthesis))
            {
                Expression expression = Expression();
                if (Peek().Type != TokenType.RightParenthesis)
                {
                    throw new ParserException(Error("Expected ')'", Peek()));
                }

                Advance();

                return new Expression.Grouping(expression);
            }

            throw new ParserException(Error("Expected expression.", Previous()));
        }
        #endregion

        #region "Statements"

        private Statement ClsStatement()
        {
            return new Statement.ClsStatement(_currentBasicLineNumber);
        }

        private Statement CursorStatement()
        {
            if (Match(TokenType.On))
            {
                return new Statement.CursorStatement(true, _currentBasicLineNumber);
            } else if (Match(TokenType.Off))
            {
                return new Statement.CursorStatement(false, _currentBasicLineNumber);
            }

            throw new ParserException(Error("Expected 'OFF' or 'ON' following 'CURSOR' statement.", Previous()));
        }

        private Statement DataStatement()
        {
            var data = new Queue<Expression>();

            while (!IsAtEnd())
            {
                Expression val = Expression();
                if (!(val is Expression.Literal))
                {
                    throw new ParserException(Error("DATA values cannot be expressions and must be literal strings or literal numbers.",
                        Previous()));
                }

                data.Enqueue(val);
                if (Peek().Type != TokenType.Comma) break;
                Advance();
            }

            return new Statement.DataStatement(data, _currentBasicLineNumber);
        }

        private Statement DimStatement()
        {
            var capacities = new List<Expression>();

            if (!Match(TokenType.Identifier))
            {
                throw new ParserException(Error("Expected identifier after 'DIM' statement.", Peek()));
            }

            Token identifier = Previous();

            if (!Match(TokenType.LeftParenthesis))
            {
                throw new ParserException(Error("Expected '(' after identifier.", Peek()));
            }

            do
            {
                Expression capacity = Expression();
                capacities.Add(capacity);
                if (!Match(TokenType.Comma))
                {
                    break;
                }
            } while (true);

            if (!Match(TokenType.RightParenthesis))
            {
                throw new ParserException(Error("Expected ')' after 'DIM' capacity.", Peek()));
            }

            return new Statement.DimStatement(identifier, capacities, _currentBasicLineNumber);
        }

        private Statement EndStatement()
        {
            return new Statement.EndStatement(_currentBasicLineNumber);
        }

        private Statement ForStatement()
        {
            Token initToken = Peek();
            if (Peek().Type != TokenType.Identifier)
            {
                throw new ParserException(Error("Expected identifier after FOR.", Peek()));
            }

            Advance();
            if (!Match(TokenType.Equal))
            {
                throw new ParserException(Error("Expected '=' after identifier.", Peek()));
            }

            Expression startExpression = Expression();

            if (!Match(TokenType.To))
            {
                throw new ParserException(Error("Expected 'TO' after expression.", Peek()));
            }

            Expression endExpression = Expression();
            Expression stepExpression;

            if (Match(TokenType.Step))
            {
                stepExpression = Expression();
            }
            else
            {
                stepExpression = new Expression.Literal((double)1);
            }

            var statement = new Statement.ForStatement(initToken, startExpression, endExpression, stepExpression,
                _currentBasicLineNumber);

            _forStack.Push(statement);

            return statement;
        }

        private Statement GosubStatement()
        {
            if (!Match(TokenType.Number))
            {
                throw new ParserException(Error("Expected number after GOSUB", Peek()));
            }

            return new Statement.GosubStatement((int)(double)Previous().Literal, _currentBasicLineNumber);
        }

        private Statement GotoStatement()
        {
            if (!Match(TokenType.Number))
            {
                throw new ParserException(Error("Expected number after GOTO", Peek()));
            }

            return new Statement.GotoStatement((int)(double)Previous().Literal, _currentBasicLineNumber);
        }

        private Statement IfStatement()
        {
            Expression condition = Expression();

            if (!Match(TokenType.Then))
            {
                throw new ParserException(Error("Expected 'THEN' after 'IF' condition", _tokens[_current]));
            }

            var statements = new List<Statement>();
            do
            {
                while (Match(TokenType.Colon)) ;
                statements.Add(Declaration());
            } while (!IsAtEnd() && Peek().Type == TokenType.Colon && Peek().Type != TokenType.Else);

            Statement thenBranch = new Statement.Block(statements, _currentBasicLineNumber);
            Statement elseClause = null;

            if (Match(TokenType.Else))
            {
                var elseStatements = new List<Statement>();

                do
                {
                    while (Match(TokenType.Colon)) ;
                    elseStatements.Add(Declaration());
                } while (!IsAtEnd() && Peek().Type == TokenType.Colon);

                elseClause = new Statement.Block(elseStatements, _currentBasicLineNumber);
            }

            return new Statement.IfStatement(condition, thenBranch, elseClause, _currentBasicLineNumber);
        }

        private Statement LocateStatement()
        {
            Expression y = Expression();

            if (!Match(TokenType.Comma))
            {
                throw new ParserException(Error("Expected comma after expression.", Previous()));
            }
            Expression x = Expression();

            return new Statement.LocateStatement(y, x, _currentBasicLineNumber);
        }

        private Statement NextStatement()
        {
            if (_forStack.Count == 0)
            {
                throw new ParserException(Error("'NEXT' without matching 'FOR'.", Peek()));
            }

            if (Match(TokenType.Identifier))
            {
                if (Previous().Lexeme != _forStack.Peek().Variable.Lexeme)
                {
                    throw new ParserException(Error("'NEXT' identifier does not match 'FOR' identifier.", Previous()));
                }
            }

            _forStack.Pop();

            return new Statement.NextStatement(_currentBasicLineNumber);
        }

        private Statement PrintStatement()
        {
            Expression value = Expression();

            if (Match(TokenType.SemiColon))
            {
                return new Statement.PrintStatement(value, false, _currentBasicLineNumber);
            }
            return new Statement.PrintStatement(value, true, _currentBasicLineNumber);
        }

        private Statement ReadStatement()
        {
            if (!Match(TokenType.Identifier))
            {
                throw new ParserException(Error("Expected identifier after 'READ' statement.", Peek()));
            }

            Token identifier = Previous();

            if (Match(TokenType.LeftParenthesis))
            {
                var indices = new List<Expression>();

                do
                {
                    Expression indexExpression = Expression();
                    indices.Add(indexExpression);
                    if (Peek().Type != TokenType.Comma) break;
                    Advance();
                } while (true);
                
                if (!Match(TokenType.RightParenthesis))
                {
                    throw new ParserException(Error("Expected ')' after array indexer expression.", Peek()));
                }

                return new Statement.ReadStatement(identifier, true, indices, _currentBasicLineNumber);
            }

            return new Statement.ReadStatement(identifier, false, null, _currentBasicLineNumber);
        }

        private Statement RestoreStatement()
        {
            return new Statement.RestoreStatement(_currentBasicLineNumber);
        }

        private Statement ReturnStatement()
        {
            return new Statement.ReturnStatement(_currentBasicLineNumber);
        }

        #endregion

        // Utility Methods
        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;

            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;

            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.Eof;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token PeekNext()
        {
            if (_tokens.Count > _current + 1) return _tokens[_current + 1];
            else return null;
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }
    }
}
