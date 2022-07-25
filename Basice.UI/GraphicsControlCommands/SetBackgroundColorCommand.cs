using System.Drawing;

namespace Basice.UI.GraphicsControlCommands
{
    public class SetBackgroundColorCommand : IGraphicsCommand
    {
        public Color Color { get; set; }

        public SetBackgroundColorCommand(Color color)
        {
            Color = color;
        }
    }
}
