using System.Collections.Generic;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;

/*
 *   program              ->  ( lineNumber declaration "\n" )* EOF ;
 *
 *   lineNumber           -> NUMBER ;
 *   declaration          -> IDENTIFIER "=" expression
 *                           | statement ;
 *   statement            -> (printStatement | clsStatement | locateStatement | rgbStatement) (":" declaration)? ;
 *
 *   clsStatement         -> "CLS" ;
 *   locateStatement      -> "LOCATE" expression "," expression ;
 *   printStatement       -> "PRINT" expression ;
 *   rgbStatement         -> "RGB" "(" expression "," expression "," expression ")" ;
 */

namespace Basice.Interpreter.Parser
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private readonly List<int> _basicLineNumbers;
        private int _current;
        private int _currentBasicLineNumber;
        private int _previousBasicLineNumber;

        public Parser(List<Token> tokens)
        {
            _basicLineNumbers = new List<int>();
            _tokens = tokens;
            _current = 0;
            _currentBasicLineNumber = -1;
            _previousBasicLineNumber = -1;
        }

        public List<Statement> Parse()
        {
            var statements = new List<Statement>();

            while (!IsAtEnd())
            {
                statements.Add(LineNumber());
                if (Peek().Type == TokenType.NewLine)
                {
                    while (!IsAtEnd() && Match(TokenType.NewLine)) ;
                } else if (Peek().Type == TokenType.Colon)
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
            if (Match(TokenType.Locate)) return LocateStatement();
            if (Match(TokenType.Print)) return PrintStatement();

            throw new ParserException(Error($"Unrecognized statement '{_tokens[_current].Lexeme}'", Previous()));
        }

        #region "Expressions"
        private Expression Expression()
        {
            return Term();
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
            if (Match(TokenType.Minus))
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

        private Statement PrintStatement()
        {
            Expression value = Expression();

            return new Statement.PrintStatement(value, _currentBasicLineNumber);
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

        private Token Previous()
        {
            return _tokens[_current - 1];
        }
    }
}
