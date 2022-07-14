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
        And, Cls, Else, End, For, Goto, If, Locate, Or, Next, Not, Print, Step, Then, To,

        // No Operation
        Nop,
        // End of File
        Eof
    }
}
