using Basice.UI.GraphicsControlCommands;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Basice.UI
{
    public class GraphicsControl : UserControl
    {
        private readonly List<IGraphicsCommand> _commands;
        private Color _currentForegroundColor;
        private Color _currentBackgroundColor;

        public Color CurrentForegroundColor
        {
            get => _currentForegroundColor;
            set
            {
                _currentForegroundColor = value;
                _commands.Add(new SetForegroundColorCommand(value));
            }
        }

        public Color CurrentBackgroundColor
        {
            get => _currentBackgroundColor;
            set
            {
                _currentBackgroundColor = value;
                _commands.Add(new SetBackgroundColorCommand(value));
            }
        }

        public Color GraphicsControlBackgroundColor { get; set; }
        public Color GraphicsControlForegroundColor { get; set; }

        public GraphicsControl()
        {
            Width = 646;
            Height = 377;

            _commands = new List<IGraphicsCommand>(10000);

            GraphicsControlBackgroundColor = Color.Black;
            GraphicsControlForegroundColor = Color.LightGray;
            CurrentForegroundColor = Color.LightGray;
            CurrentBackgroundColor = Color.Black;
        }

        public void ResetGraphics()
        {
            _commands.Clear();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Color currentForegroundColor = _currentForegroundColor;
            Color currentBackgroundColor = _currentBackgroundColor;

            using (var bitmap = new Bitmap(Width, Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    ProcessCommands(g);
                    e.Graphics.DrawImage(bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
                }
            }

            _currentForegroundColor = currentForegroundColor;
            _currentBackgroundColor = currentBackgroundColor;

            base.OnPaint(e);
        }

        private void ProcessCommands(Graphics g)
        {
            foreach (IGraphicsCommand command in _commands)
            {
                switch (command.GetType().Name)
                {
                    case nameof(ArcCommand):
                        ArcCommand cc = (ArcCommand)command;
                        g.DrawArc(new Pen(cc.Color), cc.X, cc.Y, (float)cc.Width, (float)cc.Height, (float)cc.Start, (float)cc.End);
                        break;
                    case nameof(EllipseCommand):
                        EllipseCommand ec = (EllipseCommand)command;
                        g.DrawEllipse(new Pen(ec.Color), ec.X, ec.Y, (float)ec.Width, (float)ec.Height);
                        break;
                    case nameof(LineCommand):
                        LineCommand lc = (LineCommand)command;
                        g.DrawLine(new Pen(lc.Color), lc.X1, lc.Y1, lc.X2, lc.Y2);
                        break;
                    case nameof(PointCommand):
                        PointCommand p = (PointCommand)command;
                        g.FillRectangle(new SolidBrush(p.Color), p.X, p.Y, 1, 1);
                        break;
                    case nameof(RectangleCommand):
                        RectangleCommand rc = (RectangleCommand)command;
                        g.DrawRectangle(new Pen(rc.Color), rc.X1, rc.Y1, rc.X2 >= rc.X1 ? rc.X2 - rc.X1 : rc.X1 - rc.X2,
                            rc.Y2 >= rc.Y1 ? rc.Y2 - rc.Y1 : rc.Y1 - rc.Y2);
                        break;
                    case nameof(SetBackgroundColorCommand):
                        _currentBackgroundColor = ((SetBackgroundColorCommand)command).Color;
                        break;
                    case nameof(SetForegroundColorCommand):
                        _currentForegroundColor = ((SetForegroundColorCommand)command).Color;
                        break;
                }
            }
        }

        public void DrawArc(int x, int y, double width, double height, Color color, double start, double end)
        {
            _commands.Add(new ArcCommand(x, y, width, height, color, start, end));
            Invalidate();
        }

        public void DrawEllipse(int x, int y, double width, double height, Color color)
        {
            _commands.Add(new EllipseCommand(x, y, width, height, color));
            Invalidate();
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            _commands.Add(new LineCommand(x1, y1, x2, y2, color));
            Invalidate();
        }

        public void DrawPoint(int x, int y, Color color)
        {
            _commands.Add(new PointCommand(x, y, color));
            Invalidate();
        }

        public void DrawRectangle(int x1, int y1, int x2, int y2, Color color)
        {
            _commands.Add(new RectangleCommand(x1, y1, x2, y2, color));
            Invalidate();
        }
    }
}
