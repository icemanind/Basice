﻿namespace Basice.Interpreter.Lexer
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
        And, Cls, Color, Cursor, Data, Dim, Else, End, For, Gosub, Goto, If, Input, Locate,
        Next, Not, Off, On, Or, Print, Read, Restore, Return, Step, Then, To,

        // No Operation
        Nop,
        // End of File
        Eof
    }
}
