using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Basice.UI
{
    public partial class ConsoleControl : UserControl
    {
        private const string RenderFontName = "Courier New";
        private const int ScreenSize = 80 * 25;

        private readonly List<char> _keysBuffer;
        private readonly List<string> _commandBuffer;
        private int _commandBufferIndex;
        private bool _isCursorOn;
        private readonly Font _renderFont = new Font(RenderFontName, 10, FontStyle.Regular);
        public delegate void LineEnteredDelegate(object sender, string line);

        /// <summary>
        /// This event is raised whenever text is entered into the console control
        /// </summary>
        public event LineEnteredDelegate LineEntered;

        private Color _consoleBackgroundColor;
        private Color _consoleForegroundColor;
        private int _cursorX;
        private int _cursorY;
        private CursorTypes _cursorType;

        private readonly TextBlock[] _textBlockArray;

        public bool ProcessKeys { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating whether the console should show the cursor
        /// </summary>
        public bool ShowCursor { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating whether the console should allow keyboard input
        /// </summary>
        public bool AllowInput { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating whether the console should echo any input it receives.
        /// </summary>
        public bool EchoInput { get; set; }

        //private Timer _readLineTimer;

        public Color CurrentForegroundColor { get; set; }
        public Color CurrentBackgroundColor { get; set; }

        /// <summary>
        /// Gets or Sets the current cursor type.
        /// </summary>
        public CursorTypes CursorType
        {
            get { return _cursorType; }
            set
            {
                _cursorType = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or Sets the background color of the console.
        /// Default is Black.
        /// </summary>
        public Color ConsoleBackgroundColor
        {
            get { return _consoleBackgroundColor; }
            set
            {
                _consoleBackgroundColor = value;
                BackColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or Sets the foreground color of the console.
        /// Default is light gray.
        /// </summary>
        public Color ConsoleForegroundColor
        {
            get { return _consoleForegroundColor; }
            set
            {
                _consoleForegroundColor = value;
                ForeColor = value;
                Invalidate();
            }
        }

        public ConsoleControl()
        {
            InitializeComponent();

            Width = 646;
            Height = 377;

            _cursorX = 0;
            _cursorY = 0;
            _isCursorOn = false;
            CursorType = CursorTypes.Underline;

            ConsoleBackgroundColor = Color.Black;
            ConsoleForegroundColor = Color.LightGray;
            CurrentForegroundColor = Color.LightGray;
            CurrentBackgroundColor = Color.Black;

            ShowCursor = true;
            AllowInput = true;
            EchoInput = true;
            ProcessKeys = true;

            _textBlockArray = new TextBlock[ScreenSize]; // 80 x 25

            for (int i = 0; i < ScreenSize; i++)
            {
                _textBlockArray[i].BackgroundColor = ConsoleBackgroundColor;
                _textBlockArray[i].ForegroundColor = ConsoleForegroundColor;
                _textBlockArray[i].Character = '\0';
            }

            var cursorFlashTimer = new Timer
            {
                Enabled = true,
                Interval = 500
            };
            cursorFlashTimer.Tick += CursorFlashTimerTick;

            _keysBuffer = new List<char>();
            _commandBuffer = new List<string>();
            _commandBufferIndex = 0;

            KeyPress += ConsoleControlKeyPress;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!ProcessKeys) return base.ProcessCmdKey(ref msg, keyData); 
            if (keyData == Keys.Up)
            {
                if (_commandBufferIndex <= 0 || (!AllowInput))
                    return true;

                int len = _keysBuffer.Count;
                for (int i = 0; i < len; i++)
                {
                    ConsoleControlKeyPress(this, new KeyPressEventArgs((char)8));
                }
                _keysBuffer.Clear();
                foreach (char c in _commandBuffer[_commandBufferIndex - 1])
                {
                    _keysBuffer.Add(c);
                    if (EchoInput)
                    {
                        Write(c);
                    }
                }
                _commandBufferIndex--;
                return true;
            }
            if (keyData == Keys.Down)
            {
                if ((_commandBufferIndex + 1) >= _commandBuffer.Count || (!AllowInput))
                    return true;

                int len = _keysBuffer.Count;
                for (int i = 0; i < len; i++)
                {
                    ConsoleControlKeyPress(this, new KeyPressEventArgs((char)8));
                }
                _keysBuffer.Clear();

                foreach (char c in _commandBuffer[_commandBufferIndex + 1])
                {
                    _keysBuffer.Add(c);
                    if (EchoInput)
                    {
                        Write(c);
                    }
                }

                _commandBufferIndex++;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ConsoleControlKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!AllowInput || !ProcessKeys)
            {
                return;
            }

            if (e.KeyChar == 8)
            {
                if (_keysBuffer.Count == 0)
                    return;
                if (EchoInput)
                {
                    _textBlockArray[GetIndex()].Character = '\0';
                    _cursorX--;
                    if (_cursorX < 0)
                    {
                        _cursorY--;
                        _cursorX = 79;
                        if (_cursorY < 0)
                        {
                            _cursorY++;
                            _cursorX = 0;
                        }
                    }
                    _textBlockArray[GetIndex()].Character = '\0';
                    Invalidate();
                }
                _keysBuffer.RemoveAt(_keysBuffer.Count - 1);
                return;
            }
            _keysBuffer.Add(e.KeyChar);
            if (EchoInput)
            {
                Write(e.KeyChar);
                if (e.KeyChar == '\r')
                {
                    Write('\n');
                }
            }

            if (e.KeyChar == '\r')
            {
                if (Environment.NewLine.Length == 2)
                    _keysBuffer.Add('\n');
                string s = _keysBuffer.Aggregate("", (current, c) => current + c);
                _keysBuffer.Clear();

                _commandBuffer.Add(s.Trim('\r', '\n'));
                _commandBufferIndex = _commandBuffer.Count;
                if (LineEntered != null)
                    LineEntered(this, s);
            }
            Invalidate();
        }

        private void CursorFlashTimerTick(object sender, EventArgs e)
        {
            if (!ShowCursor)
                return;
            _isCursorOn = !_isCursorOn;
            char c;
            switch (CursorType)
            {
                case CursorTypes.Block:
                    c = '\u2588';
                    break;
                case CursorTypes.Invisible:
                    c = ' ';
                    break;
                default:
                    c = '_';
                    break;
            }
            _textBlockArray[GetIndex()].Character = _isCursorOn ? c : '\0';
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int x = 0;
            int y = 0;
            int charWidth = 8;
            int charHeight = 15;

            using (var bitmap = new Bitmap(Width, Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    for (int i = 0; i < ScreenSize; i++)
                    {
                        var fc = _textBlockArray[i].Character == '\0'
                            ? _consoleForegroundColor
                            : _textBlockArray[i].ForegroundColor;
                        var bc = _textBlockArray[i].Character == '\0'
                            ? _consoleBackgroundColor
                            : _textBlockArray[i].BackgroundColor;

                        Brush bgBrush = new SolidBrush(bc);
                        Brush fgBrush = new SolidBrush(fc);

                        g.FillRectangle(bgBrush, new Rectangle(x + 2, y + 1, charWidth, charHeight));
                        g.DrawString(
                            _textBlockArray[i].Character == '\0' ? " " : _textBlockArray[i].Character.ToString(),
                            _renderFont, fgBrush, new PointF(x, y));

                        x += charWidth;
                        if (x > 79 * charWidth)
                        {
                            y += charHeight;
                            x = 0;
                        }
                    }

                    e.Graphics.DrawImage(bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
                }
            }

            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Width = 646;
            Height = 377;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        /// <summary>
        /// Sets the position of the cursor
        /// </summary>
        /// <param name="row">The row of the cursor position</param>
        /// <param name="column">The column of the cursor position</param>
        public void SetCursorPosition(int row, int column)
        {
            if (ShowCursor)
            {
                _textBlockArray[GetIndex()].Character = '\0';
            }
            _cursorX = column;
            _cursorY = row;

            Invalidate();
        }

        /// <summary>
        /// Sets the position of the cursor
        /// </summary>
        /// <param name="location">The location of the cursor position</param>
        public void SetCursorPosition(Location location)
        {
            SetCursorPosition(location.Row, location.Column);
        }

        /// <summary>
        /// Gets the position of the cursor
        /// </summary>
        /// <returns>The location of the cursor</returns>
        public Location GetCursorPosition()
        {
            return new Location { Column = _cursorX, Row = _cursorY };
        }

        /// <summary>
        /// Writes a newline (carriage return) to the console.
        /// </summary>
        public void Write()
        {
            Write(Environment.NewLine);
        }

        /// <summary>
        /// Writes a character to the console using the
        /// current foreground color and background color.
        /// </summary>
        /// <param name="c">The character to write to the console.</param>
        public void Write(char c)
        {
            Write(c, CurrentForegroundColor, CurrentBackgroundColor);
        }

        /// <summary>
        /// Writes a character to the console using the
        /// specified foreground color and background color
        /// </summary>
        /// <param name="c">The character to write to the console.</param>
        /// <param name="fgColor">The foreground color</param>
        /// <param name="bgColor">The background color</param>
        public void Write(char c, Color fgColor, Color bgColor)
        {
            if (c == 7)
            {
                Console.Beep(1000, 500);
                return;
            }

            if (c == 13)
            {
                SetCursorPosition(GetCursorPosition().Row, 0);
                return;
            }
            if (c == 10)
            {
                if (Environment.NewLine.Length == 1)
                {
                    SetCursorPosition(GetCursorPosition().Row, 0);
                }
                _cursorY++;
                if (_cursorY > 24)
                {
                    ScrollUp();
                    _cursorY = 24;
                }
                return;
            }
            _textBlockArray[GetIndex()].Character = c;
            _textBlockArray[GetIndex()].BackgroundColor = bgColor;
            _textBlockArray[GetIndex()].ForegroundColor = fgColor;
            MoveCursorPosition();

            Invalidate();
        }

        /// <summary>
        /// Writes a string to the console using the current
        /// foreground color and background color
        /// </summary>
        /// <param name="text">The string to write to the console</param>
        public void Write(string text)
        {
            Write(text, CurrentForegroundColor, CurrentBackgroundColor);
        }

        /// <summary>
        /// Writes a string to the console using the specified
        /// foreground color and background color
        /// </summary>
        /// <param name="text">The string to write to the console</param>
        /// <param name="fgColor">The foreground color</param>
        /// <param name="bgColor">The background color</param>
        public void Write(string text, Color fgColor, Color bgColor)
        {
            foreach (char c in text)
            {
                Write(c, fgColor, bgColor);
            }
            Invalidate();
        }

        private void MoveCursorPosition()
        {
            _cursorX++;
            if (_cursorX > 79)
            {
                _cursorX = 0;
                _cursorY++;
            }
            if (_cursorY > 24)
            {
                ScrollUp();
                _cursorY = 24;
            }
        }

        private int GetIndex(int row, int col)
        {
            return 80 * row + col;
        }

        private int GetIndex()
        {
            return GetIndex(_cursorY, _cursorX);
        }

        /// <summary>
        /// Scrolls the console screen window up the given.
        /// number of lines.
        /// </summary>
        /// <param name="lines">The number of lines to scroll up</param>
        public void ScrollUp(int lines)
        {
            while (lines > 0)
            {
                for (int i = 0; i < ScreenSize - 80; i++)
                {
                    _textBlockArray[i] = _textBlockArray[i + 80];
                }
                for (int i = ScreenSize - 80; i < ScreenSize; i++)
                {
                    _textBlockArray[i].Character = '\0';
                    _textBlockArray[i].BackgroundColor = ConsoleBackgroundColor;
                    _textBlockArray[i].ForegroundColor = ConsoleForegroundColor;
                }
                lines--;
            }
            Invalidate();
        }

        /// <summary>
        /// Scrolls the console screen window up one line.
        /// </summary>
        public void ScrollUp()
        {
            ScrollUp(1);
        }

        /// <summary>
        /// Clears the console screen
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < ScreenSize; i++)
            {
                _textBlockArray[i].BackgroundColor = ConsoleBackgroundColor;
                _textBlockArray[i].ForegroundColor = ConsoleForegroundColor;
                _textBlockArray[i].Character = '\0';
            }

            _cursorX = 0;
            _cursorY = 0;

            Invalidate();
        }

        /// <summary>
        /// Sets the background color at the specified location
        /// </summary>
        /// <param name="color">The background color to set</param>
        /// <param name="row">The row at which to set the background color</param>
        /// <param name="column">The column at which to set the background color</param>
        public void SetBackgroundColorAt(Color color, int row, int column)
        {
            _textBlockArray[GetIndex(row, column)].BackgroundColor = color;
            Invalidate();
        }

        /// <summary>
        /// Sets the foreground color at the specified location
        /// </summary>
        /// <param name="color">The foreground color to set</param>
        /// <param name="row">The row at which to set the foreground color</param>
        /// <param name="column">The column at which to set the foreground color</param>
        public void SetForegroundColorAt(Color color, int row, int column)
        {
            _textBlockArray[GetIndex(row, column)].ForegroundColor = color;
            Invalidate();
        }

        /// <summary>
        /// Sets the character at the specified location
        /// </summary>
        /// <param name="character">The character to set</param>
        /// <param name="row">The row at which to place the character</param>
        /// <param name="column">The column at which to place the character</param>
        public void SetCharacterAt(char character, int row, int column)
        {
            _textBlockArray[GetIndex(row, column)].Character = character;
            Invalidate();
        }
    }
}
