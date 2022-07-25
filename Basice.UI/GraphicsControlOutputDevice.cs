using System.Drawing;
using System.Threading.Tasks;
using Basice.Interpreter.Interpreter;

namespace Basice.UI
{
    public class GraphicsControlOutputDevice : IGraphicsOutput
    {
        private readonly GraphicsControl _control;

        private Color MapIntToColor(int color)
        {
            int red = (color & 0xFF0000) >> 16;
            int green = (color & 0xFF00) >> 8;
            int blue = (color & 0xFF);

            return Color.FromArgb(red, green, blue);
        }

        public bool AsyncAvailable => true;

        public void ClearScreen(int color)
        {
            throw new System.NotImplementedException();
        }

        public Task ClearScreenAsync(int color)
        {
            throw new System.NotImplementedException();
        }

        public int GetBackgroundColor()
        {
            return (_control.CurrentBackgroundColor.R << 16) | (_control.CurrentBackgroundColor.G << 8) |
                   (_control.CurrentBackgroundColor.B);
        }

        public int GetForegroundColor()
        {
            return (_control.CurrentForegroundColor.R << 16) | (_control.CurrentForegroundColor.G << 8) |
                   (_control.CurrentForegroundColor.B);
        }

        public void Point(int x, int y, int color)
        {
            throw new System.NotImplementedException();
        }

        public async Task PointAsync(int x, int y, int color)
        {
            await Task.Run(() => _control.DrawPoint(x, y, MapIntToColor(color)));
        }

        public void Reset()
        {
            _control.ResetGraphics();
        }

        public async Task ResetAsync()
        {
            await Task.Run(() => _control.ResetGraphics());
        }

        public void Screen(int number)
        {
            _control.Visible = number == 2;
        }

        public void SetBackgroundColor(int red, int green, int blue)
        {
            _control.CurrentBackgroundColor = Color.FromArgb(red, green, blue);
        }

        public void SetForegroundColor(int red, int green, int blue)
        {
            _control.CurrentForegroundColor = Color.FromArgb(red, green, blue);
        }

        public GraphicsControlOutputDevice(GraphicsControl control)
        {
            _control = control;
        }
    }
}
