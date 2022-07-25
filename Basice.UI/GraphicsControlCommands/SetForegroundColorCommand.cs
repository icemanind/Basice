using System.Drawing;

namespace Basice.UI.GraphicsControlCommands
{
    public class SetForegroundColorCommand : IGraphicsCommand
    {
        public Color Color { get; set; }

        public SetForegroundColorCommand(Color color)
        {
            Color = color;
        }
    }
}
