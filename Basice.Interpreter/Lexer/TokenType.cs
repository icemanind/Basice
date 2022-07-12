namespace Basice.Interpreter.Lexer
{
    public enum TokenType
    {
        // Literals
        Identifier, String, Number,

        // Operators
        LeftParenthesis, RightParenthesis, Minus, Plus, Slash, Star, Equal,
        SemiColon, Colon, Comma,

        // Newline
        NewLine,

        // Statements
        Cls, Print, Locate,

        // No Operation
        Nop,
        // End of File
        Eof
    }
}
