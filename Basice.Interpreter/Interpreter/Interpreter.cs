using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;
using Basice.Interpreter.Parser;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basice.Interpreter.Interpreter
{
    public class Interpreter
    {
        private readonly Queue<double> _numberData;
        private readonly Queue<string> _stringData;
        private readonly List<Statement> _statements;
        private readonly Stack<int> _gosubStack;
        private readonly ITextOutput _textOutputDevice;
        private readonly ITextInput _textInputDevice;
        private readonly IGraphicsInput _graphicsInputDevice;
        private readonly IGraphicsOutput _graphicsOutputDevice;
        private readonly Dictionary<string, object> _variables;
        private readonly Dictionary<string, ICallable> _stdLib;
        private int _currentStatementIndex;
        private bool _endHit;

        public ITextOutput TextOutputDevice => _textOutputDevice;
        public ITextInput TextInputDevice => _textInputDevice;
        public IGraphicsInput GraphicsInputDevice => _graphicsInputDevice;

        public Interpreter(List<Statement> statements, ITextOutput textOutputDevice, ITextInput textInputDevice, IGraphicsInput graphicsInputDevice, IGraphicsOutput graphicsOutputDevice)
        {
            _numberData = new Queue<double>();
            _stringData = new Queue<string>();
            _endHit = false;
            _statements = statements;
            _currentStatementIndex = 0;
            _textOutputDevice = textOutputDevice;
            _textInputDevice = textInputDevice;
            _graphicsInputDevice = graphicsInputDevice;
            _graphicsOutputDevice = graphicsOutputDevice;
            _variables = new Dictionary<string, object>();
            _gosubStack = new Stack<int>();
            _stdLib = new Dictionary<string, ICallable>
            {
                { "ABS", new Stdlib.Abs() },
                { "ASC", new Stdlib.Asc() },
                { "CHR$", new Stdlib.Chr() },
                { "COS", new Stdlib.Cos() },
                { "DAY", new Stdlib.Day() },
                { "HEX$", new Stdlib.Hex() },
                { "HOUR", new Stdlib.Hour() },
                { "INSTR", new Stdlib.Instr() },
                { "INKEY$", new Stdlib.Inkey() },
                { "INT", new Stdlib.Int() },
                { "LEFT$", new Stdlib.Left() },
                { "LEN", new Stdlib.Len() },
                { "LOG", new Stdlib.Log() },
                { "MID$", new Stdlib.Mid() },
                { "MINUTE", new Stdlib.Minute() },
                { "MONTH", new Stdlib.Month() },
                { "RGB", new Stdlib.Rgb() },
                { "RND", new Stdlib.Rnd() },
                { "RIGHT$", new Stdlib.Right() },
                { "SECOND", new Stdlib.Second() },
                { "SIN", new Stdlib.Sin() },
                { "SQR", new Stdlib.Sqr() },
                { "STR$", new Stdlib.Str() },
                { "VAL", new Stdlib.Val() },
                { "YEAR", new Stdlib.Year() }
            };
        }

        public async Task InterpretAsync()
        {
            _textOutputDevice.Screen(1);
            _graphicsOutputDevice.Screen(1);
            await _graphicsOutputDevice.ResetAsync();
            _textOutputDevice.SetBackgroundColor(0, 0, 0);
            _textOutputDevice.SetForegroundColor(200, 200, 200);
            await _textOutputDevice.ClearScreenAsync();
            _endHit = false;
            _textInputDevice.ClearBuffer();
            _graphicsInputDevice.ClearBuffer();
            _variables.Clear();
            _gosubStack.Clear();
            _numberData.Clear();
            _stringData.Clear();
            ParseDataStatements();
            while (_currentStatementIndex < _statements.Count)
            {
                if (_endHit) break;
                await ExecuteAsync(_statements[_currentStatementIndex]);
                do
                {
                    await Task.Delay(1);
                } while (_textInputDevice.IsWaitingForInput());

                _currentStatementIndex++;
            }
        }

        private void ParseDataStatements()
        {
            foreach (Statement statement in _statements)
            {
                if (statement is Statement.DataStatement)
                {
                    var dataStatement = statement as Statement.DataStatement;
                    foreach (var expression in dataStatement.Data)
                    {
                        object dataObj = Evaluate(expression);
                        if (dataObj is double)
                        {
                            _numberData.Enqueue((double)dataObj);
                        }
                        else if (dataObj is string)
                        {
                            _stringData.Enqueue((string)dataObj);
                        }
                    }
                }
            }
        }

        private async Task ExecuteAsync(Statement statement)
        {
            while (_textInputDevice.IsWaitingForInput())
            {
                await Task.Delay(1);
            }
            switch (statement.GetType().Name)
            {
                case nameof(Statement.ArcStatement):
                    await ArcAsync((Statement.ArcStatement)statement);
                    break;
                case nameof(Statement.Block):
                    await BlockAsync((Statement.Block)statement);
                    break;
                case nameof(Statement.ClsStatement):
                    await ClsAsync();
                    break;
                case nameof(Statement.ColorStatement):
                    Color((Statement.ColorStatement)statement);
                    break;
                case nameof(Statement.CursorStatement):
                    await CursorAsync((Statement.CursorStatement)statement);
                    break;
                case nameof(Statement.DataStatement):
                    break;
                case nameof(Statement.DimStatement):
                    Dim((Statement.DimStatement)statement);
                    break;
                case nameof(Statement.DrawTextStatement):
                    DrawText((Statement.DrawTextStatement)statement);
                    break;
                case nameof(Statement.EllipseStatement):
                    Ellipse((Statement.EllipseStatement)statement);
                    break;
                case nameof(Statement.EndStatement):
                    End();
                    break;
                case nameof(Statement.ForStatement):
                    await ForAsync((Statement.ForStatement)statement);
                    break;
                case nameof(Statement.GosubStatement):
                    Gosub((Statement.GosubStatement)statement);
                    break;
                case nameof(Statement.GotoStatement):
                    Goto((Statement.GotoStatement)statement);
                    break;
                case nameof(Statement.IfStatement):
                    await IfAsync((Statement.IfStatement)statement);
                    break;
                case nameof(Statement.InputStatement):
                    Input((Statement.InputStatement)statement);
                    break;
                case nameof(Statement.LineStatement):
                    await LineAsync((Statement.LineStatement)statement);
                    break;
                case nameof(Statement.LocateStatement):
                    await LocateAsync((Statement.LocateStatement)statement);
                    break;
                case nameof(Statement.PointStatement):
                    await PointAsync((Statement.PointStatement)statement);
                    break;
                case nameof(Statement.PrintStatement):
                    await Print((Statement.PrintStatement)statement);
                    break;
                case nameof(Statement.ReadStatement):
                    Read((Statement.ReadStatement)statement);
                    break;
                case nameof(Statement.RestoreStatement):
                    Restore();
                    break;
                case nameof(Statement.ReturnStatement):
                    Return((Statement.ReturnStatement)statement);
                    break;
                case nameof(Statement.ScreenStatement):
                    Screen((Statement.ScreenStatement)statement);
                    break;
                case nameof(Statement.VariableArrayStatement):
                    DefineArrayVariable((Statement.VariableArrayStatement)statement);
                    break;
                case nameof(Statement.VariableStatement):
                    DefineVariable((Statement.VariableStatement)statement);
                    break;
            }
        }

        #region "Statement Methods"

        private async Task ArcAsync(Statement.ArcStatement statement)
        {
            object xObj = Evaluate(statement.X);
            object yObj = Evaluate(statement.Y);
            object widthObj = Evaluate(statement.Width);
            object heightObj = Evaluate(statement.Height);
            object colorObj = Evaluate(statement.Color);
            object startObj = Evaluate(statement.Start);
            object endObj = Evaluate(statement.End);

            if (!(xObj is double) || !(yObj is double) || !(widthObj is double) || !(heightObj is double))
            {
                throw new RuntimeException("ARC parameters must be numbers.", statement.BasicLineNumber);
            }

            int x = (int)(double)xObj;
            int y = (int)(double)yObj;
            double width = (double)widthObj;
            double height = (double)heightObj;
            double start = startObj == null ? 0 : (double)startObj;
            double end = endObj == null ? 360 : (double)endObj;
            int color = colorObj == null ? _graphicsOutputDevice.GetForegroundColor() : (int)(double)colorObj;

            if (_graphicsOutputDevice.AsyncAvailable)
            {
                await _graphicsOutputDevice.ArcAsync(x, y, width, height, color, start, end);
            }
            else
            {
                _graphicsOutputDevice.Arc(x, y, width, height, color, start, end);
            }
        }

        private async Task BlockAsync(Statement.Block statements)
        {
            foreach (Statement statement in statements.Statements)
            {
                await ExecuteAsync(statement);
                if (_endHit) break;
            }
        }

        private async Task ClsAsync()
        {
            if (_textOutputDevice.AsyncAvailable)
            {
                await _textOutputDevice.ClearScreenAsync();
            }
            else
            {
                _textOutputDevice.ClearScreen();
            }
        }

        private void Color(Statement.ColorStatement statement)
        {
            object foreColorObj = statement.ForegroundColor == null ? null : Evaluate(statement.ForegroundColor);
            object backColorObj = statement.BackgroundColor == null ? null : Evaluate(statement.BackgroundColor);

            if (foreColorObj != null)
            {
                if (!(foreColorObj is double))
                {
                    throw new RuntimeException("'COLOR' foreground must be a number.", statement.BasicLineNumber);
                }
            }

            if (backColorObj != null)
            {
                if (!(backColorObj is double))
                {
                    throw new RuntimeException("'COLOR' background must be a number.", statement.BasicLineNumber);
                }
            }

            if (foreColorObj != null)
            {
                int fc = (int)(double)foreColorObj;
                _textOutputDevice.SetForegroundColor((fc >> 16) & 0xff, (fc >> 8) & 0xff, (fc >> 0) & 0xff);
                _graphicsOutputDevice.SetForegroundColor((fc >> 16) & 0xff, (fc >> 8) & 0xff, (fc >> 0) & 0xff);
            }

            if (backColorObj != null)
            {
                int bc = (int)(double)backColorObj;
                _textOutputDevice.SetBackgroundColor((bc >> 16) & 0xff, (bc >> 8) & 0xff, (bc >> 0) & 0xff);
                _graphicsOutputDevice.SetBackgroundColor((bc >> 16) & 0xff, (bc >> 8) & 0xff, (bc >> 0) & 0xff);
            }
        }

        private async Task CursorAsync(Statement.CursorStatement statement)
        {
            if (_textOutputDevice.AsyncAvailable)
            {
                if (statement.CursorOn)
                {
                    await _textOutputDevice.SetCursorOnAsync();
                }
                else
                {
                    await _textOutputDevice.SetCursorOffAsync();
                }
            }
            else
            {
                if (statement.CursorOn)
                {
                    _textOutputDevice.SetCursorOn();
                }
                else
                {
                    _textOutputDevice.SetCursorOff();
                }
            }
        }

        private void Dim(Statement.DimStatement statement)
        {
            List<int> capacities = new List<int>();

            foreach (var cap in statement.Capacities)
            {
                object capObj = Evaluate(cap);

                if (!(capObj is double))
                {
                    throw new RuntimeException("Array capacity must be a number.", statement.Name);
                }

                capacities.Add((int)(double)capObj);
            }

            if (_variables.ContainsKey(statement.Name.Lexeme.ToUpper()))
            {
                _variables.Remove(statement.Name.Lexeme.ToUpper());
            }

            if (statement.Name.Lexeme.EndsWith("$"))
            {
                if (capacities.Count == 1)
                {
                    var r = new string[capacities[0] + 1];
                    for (int x = 0; x < capacities[0] + 1; x++)
                    {
                        r[x] = "";
                    }

                    _variables.Add(statement.Name.Lexeme.ToUpper(), r);
                }
                else if (capacities.Count == 2)
                {
                    var r = new string[capacities[0] + 1, capacities[1] + 1];
                    for (int x = 0; x < capacities[0] + 1; x++)
                    {
                        for (int y = 0; y < capacities[1] + 1; y++)
                        {
                            r[x, y] = "";
                        }
                    }

                    _variables.Add(statement.Name.Lexeme.ToUpper(), r);
                }
            }
            else
            {
                if (capacities.Count == 1)
                {
                    var r = new double[capacities[0] + 1];
                    for (int x = 0; x < capacities[0] + 1; x++)
                    {
                        r[x] = 0;
                    }

                    _variables.Add(statement.Name.Lexeme.ToUpper(), r);
                }
                else if (capacities.Count == 2)
                {
                    var r = new double[capacities[0] + 1, capacities[1] + 1];
                    for (int x = 0; x < capacities[0] + 1; x++)
                    {
                        for (int y = 0; y < capacities[1] + 1; y++)
                        {
                            r[x, y] = 0;
                        }
                    }

                    _variables.Add(statement.Name.Lexeme.ToUpper(), r);
                }
            }
        }

        private async void DrawText(Statement.DrawTextStatement statement)
        {
            object xObj = Evaluate(statement.X);
            object yObj = Evaluate(statement.Y);
            object textObj = Evaluate(statement.Text);
            object sizeObj = Evaluate(statement.Size);
            object colorObj = Evaluate(statement.Color);

            if (!(xObj is double) || !(yObj is double) || !(sizeObj is double))
            {
                throw new RuntimeException("DRAWTEXT coordinates and size must be numbers.", statement.BasicLineNumber);
            }

            if (!(textObj is string))
            {
                throw new RuntimeException("DRAWTEXT text must be a string.");
            }

            int x = (int)(double)xObj;
            int y = (int)(double)yObj;
            string text = (string)textObj;
            double size = (double)sizeObj;
            int color = colorObj == null ? _graphicsOutputDevice.GetForegroundColor() : (int)(double)colorObj;

            if (_graphicsOutputDevice.AsyncAvailable)
            {
                await _graphicsOutputDevice.DrawTextAsync(x, y, text, size, color);
            }
            else
            {
                _graphicsOutputDevice.DrawText(x, y, text, size, color);
            }
        }

        private async void Ellipse(Statement.EllipseStatement statement)
        {
            object xObj = Evaluate(statement.X);
            object yObj = Evaluate(statement.Y);
            object widthObj = Evaluate(statement.Width);
            object heightObj = Evaluate(statement.Height);
            object colorObj = Evaluate(statement.Color);

            if (!(xObj is double) || !(yObj is double) || !(widthObj is double) || !(heightObj is double))
            {
                throw new RuntimeException("ELLIPSE parameters must be numbers.", statement.BasicLineNumber);
            }

            int x = (int)(double)xObj;
            int y = (int)(double)yObj;
            double width = (double)widthObj;
            double height = (double)heightObj;
            int color = colorObj == null ? _graphicsOutputDevice.GetForegroundColor() : (int)(double)colorObj;

            if (_graphicsOutputDevice.AsyncAvailable)
            {
                await _graphicsOutputDevice.EllipseAsync(x, y, width, height, color);
            }
            else
            {
                _graphicsOutputDevice.Ellipse(x, y, width, height, color);
            }
        }

        private void End()
        {
            _currentStatementIndex = _statements.Count;
            _endHit = true;
        }

        private async Task ForAsync(Statement.ForStatement statement)
        {
            object start = Evaluate(statement.Start);
            object end = Evaluate(statement.End);
            object step = Evaluate(statement.Step);

            if (!(start is double) || !(end is double) || !(step is double))
            {
                throw new RuntimeException("'FOR' must be a numeric expression.", statement.Variable);
            }

            if (statement.Variable.Lexeme.EndsWith("$"))
            {
                throw new RuntimeException("'FOR' variable must be numeric.", statement.Variable);
            }

            double dblStart = (double)start;
            double dblEnd = (double)end;
            double dblStep = (double)step;

            if (_variables.ContainsKey(statement.Variable.Lexeme.ToUpper()))
            {
                _variables.Remove(statement.Variable.Lexeme.ToUpper());
            }

            _variables.Add(statement.Variable.Lexeme.ToUpper(), dblStart);

            _currentStatementIndex++;
            int currentIndex = _currentStatementIndex;
            int currentNextIndex = _currentStatementIndex;

            if (dblStep < 0)
            {
                while ((double)_variables[statement.Variable.Lexeme.ToUpper()] >= dblEnd)
                {
                    if (_currentStatementIndex >= _statements.Count) break;
                    if (!(_statements[_currentStatementIndex] is Statement.NextStatement))
                    {
                        await ExecuteAsync(_statements[_currentStatementIndex]);
                        _currentStatementIndex++;
                    }

                    if (_endHit) break;

                    if (_currentStatementIndex >= _statements.Count) break;
                    if (_statements[_currentStatementIndex] is Statement.NextStatement)
                    {
                        currentNextIndex = _currentStatementIndex;
                        _currentStatementIndex = currentIndex;
                        _variables[statement.Variable.Lexeme.ToUpper()] = (double)_variables[statement.Variable.Lexeme.ToUpper()] + dblStep;
                    }
                }
                _currentStatementIndex = currentNextIndex;
            }
            else
            {
                while ((double)_variables[statement.Variable.Lexeme.ToUpper()] <= dblEnd)
                {
                    if (_currentStatementIndex >= _statements.Count) break;
                    if (!(_statements[_currentStatementIndex] is Statement.NextStatement))
                    {
                        await ExecuteAsync(_statements[_currentStatementIndex]);
                        _currentStatementIndex++;
                    }

                    if (_endHit) break;

                    if (_currentStatementIndex >= _statements.Count) break;
                    if (_statements[_currentStatementIndex] is Statement.NextStatement)
                    {
                        currentNextIndex = _currentStatementIndex;
                        _currentStatementIndex = currentIndex;
                        _variables[statement.Variable.Lexeme.ToUpper()] = (double)_variables[statement.Variable.Lexeme.ToUpper()] + dblStep;
                    }
                }
                _currentStatementIndex = currentNextIndex;
            }

        }

        private void Gosub(Statement.GosubStatement statement)
        {
            _gosubStack.Push(_currentStatementIndex);

            foreach (Statement s in _statements)
            {
                if (s.BasicLineNumber == statement.LineNumber)
                {
                    _currentStatementIndex = _statements.IndexOf(s) - 1;
                    return;
                }
            }

            throw new RuntimeException($"No such line number [{statement.LineNumber}].", statement.BasicLineNumber);
        }

        private void Goto(Statement.GotoStatement statement)
        {
            foreach (Statement s in _statements)
            {
                if (s.BasicLineNumber == statement.LineNumber)
                {
                    _currentStatementIndex = _statements.IndexOf(s) - 1;
                    return;
                }
            }

            throw new RuntimeException($"No such line number [{statement.LineNumber}].", statement.BasicLineNumber);
        }

        private async Task IfAsync(Statement.IfStatement statement)
        {
            if (IsTruthy(Evaluate(statement.Condition)))
            {
                await ExecuteAsync(statement.ThenBranch);
            }
            else if (statement.ElseBranch != null)
            {
                await ExecuteAsync(statement.ElseBranch);
            }
        }

        private void Input(Statement.InputStatement statement)
        {
            _textInputDevice.LineEntered += InputCompleted;
            _textInputDevice.Input(statement);
        }

        private void InputCompleted(string line, Statement.InputStatement statement)
        {
            line = line.Replace("\r", "").Replace("\n", "");

            if (!statement.IsArray)
            {
                if (_variables.ContainsKey(statement.Name.Lexeme.ToUpper()))
                {
                    _variables.Remove(statement.Name.Lexeme.ToUpper());
                }

                _variables.Add(statement.Name.Lexeme.ToUpper(), line);
            }
            else
            {
                var vas = new Statement.VariableArrayStatement(statement.Name,
                    statement.ArrayIndexes, new Expression.Literal(line),
                    statement.Name.Line);

                DefineArrayVariable(vas);
            }
        }

        private async Task LineAsync(Statement.LineStatement statement)
        {
            object x1Obj = Evaluate(statement.X1);
            object x2Obj = Evaluate(statement.X2);
            object y1Obj = Evaluate(statement.Y1);
            object y2Obj = Evaluate(statement.Y2);

            if (!(x1Obj is double) || !(x2Obj is double) || !(y1Obj is double) || !(y2Obj is double))
            {
                throw new RuntimeException("LINE coordinates must be numbers.", statement.BasicLineNumber);
            }

            int x1 = (int)(double)x1Obj;
            int x2 = (int)(double)x2Obj;
            int y1 = (int)(double)y1Obj;
            int y2 = (int)(double)y2Obj;

            if (statement.Color != null)
            {
                object color = Evaluate(statement.Color);

                if (!(color is double))
                {
                    throw new RuntimeException("LINE color must be a number.", statement.BasicLineNumber);
                }

                if (_graphicsOutputDevice.AsyncAvailable)
                {
                    await _graphicsOutputDevice.LineAsync(x1, y1, x2, y2, (int)(double)color);
                }
                else
                {
                    _graphicsOutputDevice.Line(x1, y1, x2, y2, (int)(double)color);
                }

                return;
            }

            if (_graphicsOutputDevice.AsyncAvailable)
            {
                await _graphicsOutputDevice.LineAsync(x1, y1, x2, y2, _graphicsOutputDevice.GetForegroundColor());
            }
            else
            {
                _graphicsOutputDevice.Line(x1, y1, x2, y2, _graphicsOutputDevice.GetForegroundColor());
            }
        }

        private async Task LocateAsync(Statement.LocateStatement statement)
        {
            object yObj = Evaluate(statement.Y);
            object xObj = Evaluate(statement.X);

            if (!(yObj is double))
            {
                throw new RuntimeException("Y value must be a number.", statement.BasicLineNumber);
            }
            if (!(xObj is double))
            {
                throw new RuntimeException("X value must be a number.", statement.BasicLineNumber);
            }
            int y = (int)(double)yObj;
            int x = (int)(double)xObj;
            if (y <= 0 || y > 24)
            {
                throw new RuntimeException("Y value must be a number between 1 and 24.", statement.BasicLineNumber);
            }

            if (x <= 0 || x > 80)
            {
                throw new RuntimeException("X value must be a number between 1 and 80.", statement.BasicLineNumber);
            }
            if (_textOutputDevice.AsyncAvailable)
            {
                await _textOutputDevice.SetCursorPositionAsync(y - 1, x - 1);
            }
            else
            {
                _textOutputDevice.SetCursorPosition(y - 1, x - 1);
            }
        }

        private async Task Print(Statement.PrintStatement statement)
        {
            object value = Evaluate(statement.Expression);
            string crlf = statement.AddCrLf ? "\r\n" : "";

            if (_textOutputDevice.AsyncAvailable)
            {
                await _textOutputDevice.PrintAsync(value + crlf);
            }
            else
            {
                _textOutputDevice.Print(value + crlf);
            }
        }

        private async Task PointAsync(Statement.PointStatement statement)
        {
            object xObj = Evaluate(statement.X);
            object yObj = Evaluate(statement.Y);

            if (!(xObj is double) || !(yObj is double))
            {
                throw new RuntimeException("POINT coordinates must be numbers.", statement.BasicLineNumber);
            }

            int x = (int)(double)xObj;
            int y = (int)(double)yObj;

            if (statement.Color != null)
            {
                object color = Evaluate(statement.Color);

                if (!(color is double))
                {
                    throw new RuntimeException("POINT color must be a number.", statement.BasicLineNumber);
                }

                if (_graphicsOutputDevice.AsyncAvailable)
                {
                    await _graphicsOutputDevice.PointAsync(x, y, (int)(double)color);
                }
                else
                {
                    _graphicsOutputDevice.Point(x, y, (int)(double)color);
                }

                return;
            }

            if (_graphicsOutputDevice.AsyncAvailable)
            {
                await _graphicsOutputDevice.PointAsync(x, y, _graphicsOutputDevice.GetForegroundColor());
            }
            else
            {
                _graphicsOutputDevice.Point(x, y, _graphicsOutputDevice.GetForegroundColor());
            }
        }

        private void Read(Statement.ReadStatement statement)
        {
            bool isString = statement.Name.Lexeme.EndsWith("$");

            if (isString && _stringData.Count == 0)
            {
                throw new RuntimeException("Not enough 'DATA' statements for 'READ' operation.", statement.Name);
            }

            if (!isString && _numberData.Count == 0)
            {
                throw new RuntimeException("Not enough 'DATA' statements for 'READ' operation.", statement.Name);
            }

            if (!statement.IsArray)
            {
                if (_variables.ContainsKey(statement.Name.Lexeme.ToUpper()))
                {
                    _variables.Remove(statement.Name.Lexeme.ToUpper());
                }

                if (isString && !statement.IsArray)
                {
                    _variables.Add(statement.Name.Lexeme.ToUpper(), _stringData.Dequeue());
                }
                else
                {
                    _variables.Add(statement.Name.Lexeme.ToUpper(), _numberData.Dequeue());
                }

                return;
            }

            if (isString)
            {
                var vas = new Statement.VariableArrayStatement(statement.Name,
                    statement.ArrayIndexes, new Expression.Literal(_stringData.Dequeue()),
                    statement.Name.Line);

                DefineArrayVariable(vas);
            }
            else
            {
                var vas = new Statement.VariableArrayStatement(statement.Name,
                    statement.ArrayIndexes, new Expression.Literal(_numberData.Dequeue()),
                    statement.Name.Line);

                DefineArrayVariable(vas);
            }
        }

        private void Restore()
        {
            _numberData.Clear();
            _stringData.Clear();
            ParseDataStatements();
        }

        private void Return(Statement.ReturnStatement statement)
        {
            if (_gosubStack.Count == 0)
            {
                throw new RuntimeException("RETURN without GOSUB encountered.", statement.BasicLineNumber);
            }

            _currentStatementIndex = _gosubStack.Pop();
        }

        private void Screen(Statement.ScreenStatement statement)
        {
            object exp = Evaluate(statement.Number);

            if (!(exp is double))
            {
                throw new RuntimeException("SCREEN number must a number, either 1 or 2.");
            }

            int number = (int)(double)exp;

            if (number != 1 && number != 2)
            {
                throw new RuntimeException("SCREEN number must a number, either 1 or 2.");
            }

            _textOutputDevice.Screen(number);
            _graphicsOutputDevice.Screen(number);
        }

        private void DefineArrayVariable(Statement.VariableArrayStatement statement)
        {
            var init = Evaluate(statement.Initializer);
            List<object> indicesObj = new List<object>();

            foreach (var i in statement.Indices)
            {
                indicesObj.Add(Evaluate(i));
            }

            List<int> indices = new List<int>();

            foreach (var o in indicesObj)
            {
                if (!(o is double))
                {
                    throw new RuntimeException("Array indices must be numeric.", statement.Name);
                }

                indices.Add((int)(double)o);
            }

            if (!_variables.ContainsKey(statement.Name.Lexeme.ToUpper()))
            {
                throw new RuntimeException("You must use 'DIM' to create an array before accessing it.",
                    statement.Name);
            }

            object arrayObj = _variables[statement.Name.Lexeme.ToUpper()];
            if (statement.Name.Lexeme.EndsWith("$"))
            {
                if (arrayObj is string[])
                {
                    string[] obj = (string[])arrayObj;

                    if (obj.Length < indices[0])
                    {
                        throw new RuntimeException("Index outside of array bounds.", statement.Name);
                    }
                    obj[indices[0]] = (string)init;
                }
                else if (arrayObj is string[,])
                {
                    string[,] obj = (string[,])arrayObj;

                    if (obj.GetLength(0) < indices[0] || obj.GetLength(1) < indices[1])
                    {
                        throw new RuntimeException("Index outside of array bounds.", statement.Name);
                    }
                    obj[indices[0], indices[1]] = (string)init;
                }
            }
            else
            {
                if (arrayObj is double[])
                {
                    double[] obj = (double[])arrayObj;

                    if (obj.Length < indices[0])
                    {
                        throw new RuntimeException("Index outside of array bounds.", statement.Name);
                    }
                    obj[indices[0]] = (double)init;
                }
                else if (arrayObj is double[,])
                {
                    double[,] obj = (double[,])arrayObj;

                    if (obj.GetLength(0) < indices[0] || obj.GetLength(1) < indices[1])
                    {
                        throw new RuntimeException("Index outside of array bounds.", statement.Name);
                    }
                    obj[indices[0], indices[1]] = (double)init;
                }
            }
        }

        private void DefineVariable(Statement.VariableStatement statement)
        {
            var init = Evaluate(statement.Initializer);

            if (_variables.ContainsKey(statement.Name.Lexeme.ToUpper()))
            {
                _variables.Remove(statement.Name.Lexeme.ToUpper());
            }

            if (statement.Name.Lexeme.EndsWith("$") && init is double)
            {
                throw new RuntimeException("You cannot assign a number to a string variable.", statement.Name);
            }
            if (!statement.Name.Lexeme.EndsWith("$") && init is string)
            {
                throw new RuntimeException("You cannot assign a string to a number variable.", statement.Name);
            }

            _variables.Add(statement.Name.Lexeme.ToUpper(), init);
        }
        #endregion

        #region "Evaluation Methods"

        private object Evaluate(Expression expression)
        {
            if (expression == null) return null;

            switch (expression.GetType().Name)
            {
                case nameof(Expression.Binary):
                    return EvaluateBinary((Expression.Binary)expression);
                case nameof(Expression.Call):
                    return EvaluateCall((Expression.Call)expression);
                case nameof(Expression.Grouping):
                    return EvaluateGrouping((Expression.Grouping)expression);
                case nameof(Expression.Literal):
                    return EvaluateLiteral((Expression.Literal)expression);
                case nameof(Expression.Logical):
                    return EvaluateLogical((Expression.Logical)expression);
                case nameof(Expression.Unary):
                    return EvaluateUnary((Expression.Unary)expression);
                case nameof(Expression.Variable):
                    return EvaluateVariable((Expression.Variable)expression);
            }

            return null;
        }

        private object EvaluateBinary(Expression.Binary expression)
        {
            object left = Evaluate(expression.Left);
            object right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.GreaterThan:
                    return (double)left > (double)right ? (double)1 : (double)0;
                case TokenType.GreaterThanOrEqual:
                    return (double)left >= (double)right ? (double)1 : (double)0;
                case TokenType.LessThan:
                    return (double)left < (double)right ? (double)1 : (double)0;
                case TokenType.LessThanOrEqual:
                    return (double)left <= (double)right ? (double)1 : (double)0;
                case TokenType.NotEqual:
                    return !IsEqual(left, right) ? (double)1 : (double)0;
                case TokenType.Equal:
                    return IsEqual(left, right) ? (double)1 : (double)0;
                case TokenType.Plus:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }

                    throw new RuntimeException("Operands must be two numbers or two strings.", expression.Operator);
                case TokenType.Minus:
                    IsNumberOperands(expression.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.Star:
                    IsNumberOperands(expression.Operator, left, right);
                    return (double)left * (double)right;
                case TokenType.Slash:
                    IsNumberOperands(expression.Operator, left, right);
                    IsDivideByZeroOperand(expression.Operator, right);
                    return (double)left / (double)right;
            }

            return null;
        }

        private object EvaluateCall(Expression.Call expression)
        {
            object callee = Evaluate(expression.Callee);

            var arguments = new List<object>();

            foreach (Expression argument in expression.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is ICallable) && !(callee is double[]) && !(callee is string[]) && !(callee is double[,]) && !(callee is string[,]))
            {
                throw new RuntimeException("You can only call functions.", expression.Paren);
            }

            if (callee is double[])
            {
                return ((double[])callee)[(int)(double)arguments[0]];
            }
            else if (callee is string[])
            {
                return ((string[])callee)[(int)(double)arguments[0]];
            }
            else if (callee is double[,])
            {
                return ((double[,])callee)[(int)(double)arguments[0], (int)(double)arguments[1]];
            }
            else if (callee is string[,])
            {
                return ((string[,])callee)[(int)(double)arguments[0], (int)(double)arguments[1]];
            }

            var function = (ICallable)callee;

            if (arguments.Count != function.NumberOfArguments())
            {
                throw new RuntimeException(
                    $"Expected {function.NumberOfArguments()} arguments, but got {arguments.Count} instead.",
                    expression.Paren);
            }

            return function.Call(this, expression.Paren, arguments);
        }

        private object EvaluateGrouping(Expression.Grouping expression)
        {
            return Evaluate(expression.Expression);
        }

        private object EvaluateLiteral(Expression.Literal expression)
        {
            return expression.Value;
        }

        private object EvaluateLogical(Expression.Logical expression)
        {
            object left = Evaluate(expression.Left);

            if (expression.Operator.Type == TokenType.Or)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expression.Right);
        }

        private object EvaluateUnary(Expression.Unary expression)
        {
            object right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.Minus:
                    IsNumberOperand(expression.Operator, right);
                    return -(double)right;
                case TokenType.Not:
                    IsNumberOperand(expression.Operator, right);
                    return ((double)right).Equals(1) ? (double)0 : (double)1;
            }

            return null;
        }

        private object EvaluateVariable(Expression.Variable expression)
        {
            if (_variables.ContainsKey(expression.Name.Lexeme.ToUpper()))
            {
                return _variables[expression.Name.Lexeme.ToUpper()];
            }

            if (_stdLib.ContainsKey(expression.Name.Lexeme.ToUpper()))
            {
                return _stdLib[expression.Name.Lexeme.ToUpper()];
            }

            if (expression.Name.Lexeme.EndsWith("$"))
            {
                _variables.Add(expression.Name.Lexeme.ToUpper(), "");
                return "";
            }
            else
            {
                _variables.Add(expression.Name.Lexeme.ToUpper(), (double)0);
                return (double)0;
            }
        }
        #endregion

        #region "Helper Methods"
        private void IsNumberOperand(Token @operator, object operand)
        {
            if (operand is double)
            {
                return;
            }

            throw new RuntimeException("Operand must be a number.", @operator);
        }

        private void IsNumberOperands(Token @operator, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeException("Operands must be numbers.", @operator);
        }

        private void IsDivideByZeroOperand(Token @operator, object operand)
        {
            if (operand is double && (double)operand == 0)
            {
                throw new RuntimeException("You cannot divide by zero.", @operator);
            }
        }

        private bool IsEqual(object obj1, object obj2)
        {
            return obj1.Equals(obj2);
        }

        private bool IsTruthy(object obj)
        {
            if (obj is double) return ((double)obj).Equals(1);

            throw new System.Exception();
        }
        #endregion
    }
}
