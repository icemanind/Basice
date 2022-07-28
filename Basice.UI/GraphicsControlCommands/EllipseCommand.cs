using System.Drawing;

namespace Basice.UI.GraphicsControlCommands
{
    public class EllipseCommand : IGraphicsCommand
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Color Color { get; set; }

        public EllipseCommand(int x,int y, double width, double height, Color color)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
        }
    }
}
