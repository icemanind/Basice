using System.Drawing;

namespace Basice.UI.GraphicsControlCommands
{
    public class DrawTextCommand : IGraphicsCommand
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }
        public string Text { get; set; }
        public double Size { get; set; }

        public DrawTextCommand(int x, int y, string text, double size, Color color)
        {
            X = x;
            Y = y;
            Text = text;
            Size = size;
            Color = color;
        }
    }
}
