using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Basice.Interpreter.Interpreter;
using Basice.Interpreter.Parser;

namespace Basice.UI
{
    public class ConsoleControlInputDevice : ITextInput
    {
        private readonly ConsoleControl _control;
        private readonly Queue<char> _keyBuffer;
        private bool _isWaitingForInput;
        private Statement.InputStatement _statement;

        public event LineEnteredDelegate LineEntered;

        public ConsoleControlInputDevice(ConsoleControl control)
        {
            _keyBuffer = new Queue<char>();

            _control = control;
            _control.AllowInput = true;
            _control.EchoInput = false;
            _control.ProcessKeys = false;
            _isWaitingForInput = false;

            _control.KeyPress += ControlKeyPressed;
        }

        private void ControlLineEntered(object sender, string line)
        {
            _control.EchoInput = false;
            _control.ProcessKeys = false;
            
            LineEntered?.Invoke(line, _statement);
            _isWaitingForInput = false;
        }

        private void ControlKeyPressed(object sender, KeyPressEventArgs e)
        {
            _keyBuffer.Enqueue(e.KeyChar);
        }

        public bool IsWaitingForInput()
        {
            return _isWaitingForInput;
        }

        public void ClearBuffer()
        {
            _keyBuffer.Clear();
        }

        public char GetNextChar()
        {
            return _keyBuffer.Count == 0 ? '\0' : _keyBuffer.Dequeue();
        }

        public void Input(Statement.InputStatement statement)
        {
            _statement = statement;
            _control.EchoInput = true;
            _control.ProcessKeys = true;
            _isWaitingForInput = true;
            _control.LineEntered += ControlLineEntered;
        }
    }
}
