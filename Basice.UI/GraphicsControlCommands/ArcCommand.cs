using System.Drawing;

namespace Basice.UI.GraphicsControlCommands
{
    public class ArcCommand : IGraphicsCommand
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Color Color { get; set; }
        public double Start { get; set; }
        public double End { get; set; }

        public ArcCommand(int x, int y, double width, double height, Color color, double start, double end)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
            Start = start;
            End = end;
        }
    }
}
