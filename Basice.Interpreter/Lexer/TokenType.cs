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
        And, Arc, Cls, Color, Cursor, Data, Dim, Ellipse, Else, End, For, Gosub, 
        Goto, If, Input, Line, Locate, Next, Not, Off, On, Or, Point, Print, Read, 
        Restore, Return, Screen, Step, Then, To,

        // No Operation
        Nop,
        // End of File
        Eof
    }
}
