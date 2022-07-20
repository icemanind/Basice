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
        And, Cls, Data, Dim, Cursor, Else, End, For, Gosub, Goto, If, Input, Locate,
        Off, On, Or, Next, Not, Print, Read, Restore, Return, Step, Then, To,

        // No Operation
        Nop,
        // End of File
        Eof
    }
}
