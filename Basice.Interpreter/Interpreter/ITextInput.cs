using Basice.Interpreter.Parser;

namespace Basice.Interpreter.Interpreter
{
    public delegate void LineEnteredDelegate(string line, Statement.InputStatement statement);

    public interface ITextInput
    {
        event LineEnteredDelegate LineEntered;
        bool IsWaitingForInput();
        void ClearBuffer();
        char GetNextChar();
        void Input(Statement.InputStatement statement);
    }
}
