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

            txtProgram.Text = "10 SCREEN 2" + Environment.NewLine + "20 ELLIPSE 10,15,30,30,RGB(255,0,0)";
            //consoleProgram.Visible = false;
            //graphicsControl1.Visible = true;
            //graphicsControl1.DrawArc(128, 96, 10, 20, Color.White, 0, 360);
            //graphicsControl1.DrawEllipse(200, 200, 50, 50, Color.Aquamarine);
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
                var graphicsOutputDevice = new GraphicsControlOutputDevice(graphicsControl1);
                var interpreter =
                    new Interpreter.Interpreter.Interpreter(statements, outputDevice, inputDevice,
                        graphicsOutputDevice);

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
