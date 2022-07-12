using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basice.Interpreter.Interpreter;

namespace Basice.UI
{
    public class ConsoleControlOutputDevice : ITextOutput
    {
        private readonly ConsoleControl _control;
        private int _backgroundColor;
        private int _foregroundColor;
        private int _cursorX;
        private int _cursorY;

        public bool AsyncAvailable => true;

        public ConsoleControlOutputDevice(ConsoleControl control)
        {
            _control = control;
            _cursorX = 0;
            _cursorY = 0;
            _backgroundColor = ConsoleColor.Black;
            _foregroundColor = ConsoleColor.Gray;
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
            _control.ConsoleBackgroundColor = MapIntToColor(_backgroundColor);
            await Task.Run(() => _control.Clear()); 
        }

        public int GetBackgroundColor()
        {
            return _backgroundColor;
        }

        public CursorLocation GetCursorPosition()
        {
            return new CursorLocation(_cursorY, _cursorX);
        }

        public int GetForegroundColor()
        {
            return _foregroundColor;
        }

        public void Print(string text)
        {
            throw new NotImplementedException();
        }

        public void Print(string text, int locationY, int locationX)
        {
            throw new NotImplementedException();
        }

        public void Print(string text, int locationY, int locationX, int foregroundColor, int backgroundColor)
        {
            throw new NotImplementedException();
        }

        public async Task PrintAsync(string text)
        {
            await PrintAsync(text, _cursorY, _cursorX);
        }

        public async Task PrintAsync(string text, int locationY, int locationX)
        {
            await PrintAsync(text, locationY, locationX, _foregroundColor, _backgroundColor);
        }

        public async Task PrintAsync(string text, int locationY, int locationX, int foregroundColor, int backgroundColor)
        {
            _control.SetCursorPosition(locationY, locationX);
            await Task.Run(() => _control.Write(text, MapIntToColor(foregroundColor), MapIntToColor(backgroundColor)));
            _cursorX = _control.GetCursorPosition().Column;
            _cursorY = _control.GetCursorPosition().Row;
        }

        public void SetBackgroundColor(int color)
        {
            _backgroundColor = color;
        }

        public void SetCursorPosition(int y, int x)
        {
            _cursorX = x;
            _cursorY = y;
        }

        public void SetForegroundColor(int color)
        {
            _foregroundColor = color;
        }
    }
}
