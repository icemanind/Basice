using System.Collections.Generic;
using System.Windows.Forms;
using Basice.Interpreter.Interpreter;

namespace Basice.UI
{
    public class ConsoleControlInputDevice : ITextInput
    {
        private readonly ConsoleControl _control;
        private readonly Queue<char> _keyBuffer;

        public ConsoleControlInputDevice(ConsoleControl control)
        {
            _keyBuffer = new Queue<char>();

            _control = control;
            _control.AllowInput = true;
            _control.EchoInput = false;
            _control.ProcessKeys = false;

            _control.KeyPress += ControlKeyPressed;
        }

        private void ControlKeyPressed(object sender, KeyPressEventArgs e)
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
