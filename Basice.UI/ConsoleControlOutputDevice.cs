using System;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using Basice.Interpreter.Interpreter;

namespace Basice.UI
{
    public class ConsoleControlOutputDevice : ITextOutput
    {
        private readonly ConsoleControl _control;

        public bool AsyncAvailable => true;

        public ConsoleControlOutputDevice(ConsoleControl control)
        {
            _control = control;
        }
        
        private Color MapIntToColor(int color)
        {
            int red = (color & 0xFF0000) >> 16;
            int green = (color & 0xFF00) >> 8;
            int blue = (color & 0xFF);

            return Color.FromArgb(red, green, blue);
        }

        public void ClearScreen(int color)
        {
            throw new NotImplementedException();
        }

        public async Task ClearScreenAsync(int color)
        {
            _control.ConsoleBackgroundColor = MapIntToColor(color);
            await ClearScreenAsync();
        }

        public void ClearScreen()
        {
            throw new NotImplementedException();
        }

        public async Task ClearScreenAsync()
        {
            _control.ConsoleBackgroundColor = _control.CurrentBackgroundColor;
            await Task.Run(() => _control.Clear()); 
        }

        public int GetBackgroundColor()
        {
            string hex = $"{_control.CurrentBackgroundColor.R:x2}{_control.CurrentBackgroundColor.G:x2}{_control.CurrentBackgroundColor.B:x2}";

            return int.Parse(hex, NumberStyles.HexNumber);
        }

        public CursorLocation GetCursorPosition()
        {
            return new CursorLocation(_control.GetCursorPosition().Row , _control.GetCursorPosition().Column );
        }

        public int GetForegroundColor()
        {
            string hex = $"{_control.CurrentForegroundColor.R:x2}{_control.CurrentForegroundColor.G:x2}{_control.CurrentForegroundColor.B:x2}";

            return int.Parse(hex, NumberStyles.HexNumber);
        }

        public int GetScreen()
        {
            return _control.Visible ? 1 : 2;
        }

        public void Print(string text)
        {
            throw new NotImplementedException();
        }

        public async Task PrintAsync(string text)
        {
            await Task.Run(() => _control.Write(text, _control.CurrentForegroundColor, _control.CurrentBackgroundColor));
        }

        public void Screen(int number)
        {
            _control.Visible = number == 1;

            if (_control.Visible) _control.Focus();
        }

        public void SetBackgroundColor(int red, int green, int blue)
        {
            _control.CurrentBackgroundColor = Color.FromArgb(red, green, blue);
        }

        public Task SetCursorOffAsync()
        {
            _control.ShowCursor = false;
            return Task.CompletedTask;
        }

        public void SetCursorOff()
        {
            throw new NotImplementedException();
        }

        public Task SetCursorOnAsync()
        {
            _control.ShowCursor = true;
            return Task.CompletedTask;
        }

        public void SetCursorOn()
        {
            throw new NotImplementedException();
        }

        public void SetCursorPosition(int y, int x)
        {
            _control.SetCursorPosition(y, x);
        }

        public async Task SetCursorPositionAsync(int y, int x)
        {
            await Task.Run(() => _control.SetCursorPosition(y,x));
        }

        public void SetForegroundColor(int red, int green, int blue)
        {
            _control.CurrentForegroundColor = Color.FromArgb(red, green, blue);
        }
    }
}
