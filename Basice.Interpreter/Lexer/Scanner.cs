using System.Collections.Generic;
using System.Globalization;
using Basice.Interpreter.Exceptions;

namespace Basice.Interpreter.Lexer
{
    public class Scanner
    {
        private string _program;
        private readonly List<Token> _tokens;
        private readonly Dictionary<string, TokenType> _keywords;
        private int _start;
        private int _current;
        private int _line;

        public Scanner(string program)
        {
            _program = program + "\n";
            _keywords = new Dictionary<string, TokenType>();
            _tokens = new List<Token>();

            _start = 0;
            _current = 0;
            _line = 1;

            _keywords.Add("AND", TokenType.And);
            _keywords.Add("ARC", TokenType.Arc);
            _keywords.Add("CLS", TokenType.Cls);
            _keywords.Add("COLOR", TokenType.Color);
            _keywords.Add("CURSOR", TokenType.Cursor);
            _keywords.Add("DATA", TokenType.Data);
            _keywords.Add("DIM", TokenType.Dim);
            _keywords.Add("DRAWTEXT", TokenType.DrawText);
            _keywords.Add("ELLIPSE", TokenType.Ellipse);
            _keywords.Add("ELSE", TokenType.Else);
            _keywords.Add("END", TokenType.End);
            _keywords.Add("FOR", TokenType.For);
            _keywords.Add("GOSUB", TokenType.Gosub);
            _keywords.Add("GOTO", TokenType.Goto);
            _keywords.Add("IF", TokenType.If);
            _keywords.Add("INPUT", TokenType.Input);
            _keywords.Add("LINE", TokenType.Line);
            _keywords.Add("LOCATE", TokenType.Locate);
            _keywords.Add("NEXT", TokenType.Next);
            _keywords.Add("NOT", TokenType.Not);
            _keywords.Add("OFF", TokenType.Off);
            _keywords.Add("ON", TokenType.On);
            _keywords.Add("OR", TokenType.Or);
            _keywords.Add("POINT", TokenType.Point);
            _keywords.Add("PRINT", TokenType.Print);
            _keywords.Add("READ", TokenType.Read);
            _keywords.Add("RESTORE", TokenType.Restore);
            _keywords.Add("RETURN", TokenType.Return);
            _keywords.Add("SCREEN", TokenType.Screen);
            _keywords.Add("STEP", TokenType.Step);
            _keywords.Add("THEN", TokenType.Then);
            _keywords.Add("TO", TokenType.To);
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.Eof, "", null, _line));
            return _tokens;
        }

        private void ScanToken()
        {
            char c = Advance();

            switch (c)
            {
                case '(': AddToken(TokenType.LeftParenthesis); break;
                case ')': AddToken(TokenType.RightParenthesis); break;
                case '+': AddToken(TokenType.Plus); break;
                case '-': AddToken(TokenType.Minus); break;
                case '*': AddToken(TokenType.Star); break;
                case '/': AddToken(TokenType.Slash); break;
                case '=': AddToken(TokenType.Equal); break;
                case ':': AddToken(TokenType.Colon); break;
                case ';': AddToken(TokenType.SemiColon); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.':
                    if (IsDigit(Peek()))
                    {
                        _program = _program.Insert(_current - 1, "0");
                        _current--;
                    }
                    break;
                case '<':
                    if (Peek() == '=')
                    {
                        _current++;
                        AddToken(TokenType.LessThanOrEqual);
                        break;
                    }

                    if (Peek() == '>')
                    {
                        _current++;
                        AddToken(TokenType.NotEqual);
                        break;
                    }

                    AddToken(TokenType.LessThan);
                    break;
                case '>':
                    if (Peek() == '=')
                    {
                        _current++;
                        AddToken(TokenType.GreaterThanOrEqual);
                        break;
                    }

                    AddToken(TokenType.GreaterThan);
                    break;

                case '\'':
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                    AddToken(TokenType.Nop);
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break;

                case '\n':
                    AddToken(TokenType.NewLine);
                    _line++;
                    break;

                case '\"': String(); break;

                default:
                    if (c == '&' && (Peek() == 'H' || Peek() == 'h'))
                    {
                        HexNumber();
                    }
                    else if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    break;
            }
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                Advance();
            }

            if (IsAtEnd())
            {
                throw new LexerException($"Line [{_line}]: Unterminated string.");
            }

            Advance();

            string value = _program.Substring(_start + 1, _current - _start - 2);
            AddToken(TokenType.String, value);
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.Number, double.Parse(_program.Substring(_start, _current - _start)));
        }

        private void HexNumber()
        {
            Advance();

            while (IsHexNumber(Peek())) Advance();

            string hexNumber = _program.Substring(_start, _current - _start);
            AddToken(TokenType.Number, (double)int.Parse(hexNumber.Remove(0, 2), NumberStyles.HexNumber));
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()) || (Peek() == '$' && !IsAlphaNumeric(PeekNext()))) Advance();

            string text = _program.Substring(_start, _current - _start);
            TokenType type = !_keywords.ContainsKey(text.ToUpper()) ? TokenType.Identifier : _keywords[text.ToUpper()];
            AddToken(type);
        }

        // Utility Functions
        private bool IsAtEnd()
        {
            return _current >= _program.Length;
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool IsHexNumber(char c)
        {
            return (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || IsDigit(c);
        }

        private char Advance()
        {
            return _program[_current++];
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return _program[_current];
        }

        private char PeekNext()
        {
            if (_current + 1 >= _program.Length) return '\0';
            return _program[_current + 1];
        }

        private char PeekNextNext()
        {
            if (_current + 2 >= _program.Length) return '\0';
            return _program[_current + 2];
        }

        private void AddToken(TokenType type, object literal = null)
        {
            string text = _program.Substring(_start, _current - _start);

            _tokens.Add(new Token(type, text, literal, _line));
        }
    }
}
