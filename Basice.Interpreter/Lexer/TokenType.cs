namespace Basice.Interpreter.Lexer
{
    public enum TokenType
    {
        // Literals
        Identifier, String, Number,

        // Operators
        LeftParenthesis, RightParenthesis, Minus, Plus, Slash, Star, Equal,
        SemiColon, Colon, Comma, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual,
        NotEqual,

        // Newline
        NewLine,

        // Statements
        And, Cls, Else, End, Goto, If, Locate, Or, Not, Print, Then, 

        // No Operation
        Nop,
        // End of File
        Eof
    }
}
