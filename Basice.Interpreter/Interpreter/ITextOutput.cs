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
        int GetScreen();
        void Print(string text);
        Task PrintAsync(string text);
        void Screen(int number);
        void SetBackgroundColor(int red, int green, int blue);
        Task SetCursorOffAsync();
        void SetCursorOff();
        Task SetCursorOnAsync();
        void SetCursorOn();
        void SetCursorPosition(int y, int x);
        Task SetCursorPositionAsync(int y, int x);
        void SetForegroundColor(int red, int green, int blue);
    }
}
