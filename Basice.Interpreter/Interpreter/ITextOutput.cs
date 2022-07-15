using System.Threading.Tasks;

namespace Basice.Interpreter.Interpreter
{
    public interface ITextOutput
    {
        bool AsyncAvailable { get; }
        void ClearScreen(int color);
        Task ClearScreenAsync(int color);
        void ClearScreen();
        Task ClearScreenAsync();
        int GetBackgroundColor();
        CursorLocation GetCursorPosition();
        int GetForegroundColor();
        void Print(string text);
        void Print(string text, int locationY, int locationX);
        void Print(string text, int locationY, int locationX, int foregroundColor, int backgroundColor);
        Task PrintAsync(string text);
        Task PrintAsync(string text, int locationY, int locationX);
        Task PrintAsync(string text, int locationY, int locationX, int foregroundColor, int backgroundColor);
        void SetBackgroundColor(int color);
        Task SetCursorOffAsync();
        void SetCursorOff();
        Task SetCursorOnAsync();
        void SetCursorOn();
        void SetCursorPosition(int y, int x);
        void SetForegroundColor(int color);
    }
}
