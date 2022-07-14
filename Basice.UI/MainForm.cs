using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basice.Interpreter.Lexer;
using Basice.Interpreter.Parser;

namespace Basice.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            txtProgram.Text = "10 CLS" + Environment.NewLine + "20 FOR x=1 to 5" + Environment.NewLine +
                              "30 PRINT x" + Environment.NewLine + "40 NEXT";
        }

        private async void BtnRunProgram_Click(object sender, EventArgs e)
        {
            consoleProgram.Focus();
            var scanner = new Scanner(txtProgram.Text);
            List<Token> tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            List<Statement> statements = parser.Parse();

            var outputDevice = new ConsoleControlOutputDevice(consoleProgram);
            var interpreter = new Interpreter.Interpreter.Interpreter(statements, outputDevice);

            await interpreter.InterpretAsync();
        }
    }
}
