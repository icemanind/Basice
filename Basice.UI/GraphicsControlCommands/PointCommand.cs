using System.Drawing;

namespace Basice.UI.GraphicsControlCommands
{
    public class PointCommand : IGraphicsCommand
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }

        public PointCommand(int x, int y, Color color)
        {
            X = x;
            Y = y;
            Color = color;
        }
    }
}
