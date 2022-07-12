using System.Collections.Generic;
using System.Globalization;
using Basice.Interpreter.Exceptions;

namespace Basice.Interpreter.Lexer
{
    public class Scanner
    {
        private readonly string _program;
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

            _keywords.Add("CLS", TokenType.Cls);
            _keywords.Add("LOCATE", TokenType.Locate);
            _keywords.Add("PRINT", TokenType.Print);
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme.
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
