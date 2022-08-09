using System.Collections.Generic;
using Basice.Interpreter.Interpreter;

namespace Basice.UI
{
    public class GraphicsControlInputDevice : IGraphicsInput
    {
        private readonly Queue<char> _keyBuffer;
        private readonly GraphicsControl _control;

        public GraphicsControlInputDevice(GraphicsControl control)
        {
            _keyBuffer = new Queue<char>();

            _control = control;

            _control.KeyPress += ControlKeyPressed;
        }

        private void ControlKeyPressed(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            _keyBuffer.Enqueue(e.KeyChar);
        }

        public void ClearBuffer()
        {
            _keyBuffer.Clear();
        }

        public char GetNextChar()
        {
            return _keyBuffer.Count == 0 ? '\0' : _keyBuffer.Dequeue();
        }
    }
}
