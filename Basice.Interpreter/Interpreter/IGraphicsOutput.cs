using System.Threading.Tasks;

namespace Basice.Interpreter.Interpreter
{
    public interface IGraphicsOutput
    {
        bool AsyncAvailable { get; }
        void ClearScreen(int color);
        Task ClearScreenAsync(int color);
        int GetBackgroundColor();
        int GetForegroundColor();
        void Point(int x, int y, int color);
        Task PointAsync(int x, int y, int color);
        void Reset();
        Task ResetAsync();
        void Screen(int number);
        void SetBackgroundColor(int red, int green, int blue);
        void SetForegroundColor(int red, int green, int blue);
    }
}
