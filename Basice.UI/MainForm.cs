using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;
using Basice.Interpreter.Parser;

namespace Basice.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private async void BtnRunProgram_Click(object sender, EventArgs e)
        {
            consoleProgram.Focus();
            TxtErrors.Text = "";
            try
            {
                var scanner = new Scanner(txtProgram.Text);
                List<Token> tokens = scanner.ScanTokens();

                var parser = new Parser(tokens);
                List<Statement> statements = parser.Parse();

                consoleProgram.Reset();
                var outputDevice = new ConsoleControlOutputDevice(consoleProgram);
                var inputDevice = new ConsoleControlInputDevice(consoleProgram);
                var graphicsInputDevice = new GraphicsControlInputDevice(graphicsControl1);
                var graphicsOutputDevice = new GraphicsControlOutputDevice(graphicsControl1);
                var interpreter =
                    new Interpreter.Interpreter.Interpreter(statements, outputDevice, inputDevice,
                        graphicsInputDevice, graphicsOutputDevice);

                await interpreter.InterpretAsync();
            }
            catch (ParserException pex)
            {
                TxtErrors.Text += $"Parser Error: {pex.Message}";
            }
            catch (RuntimeException rex)
            {
                TxtErrors.Text += $"Runtime Error: {rex.Message}";
            }
        }

        private void BtnLoadExample_Click(object sender, EventArgs e)
        {
            var examplesBrowser = new ExamplesBrowser();
            examplesBrowser.ShowDialog(this);

            if (examplesBrowser.Program != "") txtProgram.Text = examplesBrowser.Program;
        }
    }
}
