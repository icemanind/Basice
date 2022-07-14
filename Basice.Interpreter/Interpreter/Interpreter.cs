using System.Collections.Generic;
using System.Threading.Tasks;
using Basice.Interpreter.Exceptions;
using Basice.Interpreter.Lexer;
using Basice.Interpreter.Parser;

namespace Basice.Interpreter.Interpreter
{
    public class Interpreter
    {
        private readonly List<Statement> _statements;
        private readonly ITextOutput _textOutputDevice;
        private readonly Dictionary<string, object> _variables;
        private readonly Dictionary<string, ICallable> _stdLib;
        private int _currentStatementIndex;
        private readonly Stack<Statement.ForStatement> _forStack;

        public Interpreter(List<Statement> statements, ITextOutput textOutputDevice)
        {
            _forStack = new Stack<Statement.ForStatement>();
            _statements = statements;
            _currentStatementIndex = 0;
            _textOutputDevice = textOutputDevice;
            _variables = new Dictionary<string, object>();
            _stdLib = new Dictionary<string, ICallable>
            {
                { "ABS", new Stdlib.Abs() },
                { "ASC", new Stdlib.Asc() },
                { "CHR$", new Stdlib.Chr() },
                { "HEX$", new Stdlib.Hex() },
                { "INT", new Stdlib.Int() },
                { "LEFT$", new Stdlib.Left() },
                { "LEN", new Stdlib.Len() },
                { "MID$", new Stdlib.Mid() },
                { "RGB", new Stdlib.Rgb() },
                { "RND", new Stdlib.Rnd() },
                { "RIGHT$", new Stdlib.Right() },
                { "STR$", new Stdlib.Str() },
                { "VAL", new Stdlib.Val() }
            };
        }

        public async Task InterpretAsync()
        {
            while (_currentStatementIndex < _statements.Count)
            {
                await ExecuteAsync(_statements[_currentStatementIndex]);
                _currentStatementIndex++;
            }
        }

        private async Task ExecuteAsync(Statement statement)
        {
            switch (statement.GetType().Name)
            {
                case nameof(Statement.Block):
                    await BlockAsync((Statement.Block)statement);
                    break;
                case nameof(Statement.ClsStatement):
                    await ClsAsync();
                    break;
                case nameof(Statement.EndStatement):
                    End();
                    break;
                case nameof(Statement.ForStatement):
                    await ForAsync((Statement.ForStatement)statement);
                    break;
                case nameof(Statement.GotoStatement):
                    Goto((Statement.GotoStatement)statement);
                    break;
                case nameof(Statement.IfStatement):
                    await IfAsync((Statement.IfStatement)statement);
                    break;
                case nameof(Statement.LocateStatement):
                    Locate((Statement.LocateStatement)statement);
                    break;
                case nameof(Statement.PrintStatement):
                    await Print((Statement.PrintStatement)statement);
                    break;
                case nameof(Statement.VariableStatement):
                    DefineVariable((Statement.VariableStatement)statement);
                    break;
            }
        }

        #region "Statement Methods"

        private async Task BlockAsync(Statement.Block statements)
        {
            foreach (Statement statement in statements.Statements)
            {
                await ExecuteAsync(statement);
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

        private void End()
        {
            _currentStatementIndex = _statements.Count;
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
                    await ExecuteAsync(_statements[_currentStatementIndex]);
                    _currentStatementIndex++;

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
                    await ExecuteAsync(_statements[_currentStatementIndex]);
                    _currentStatementIndex++;

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
            } else if (statement.ElseBranch != null)
            {
                await ExecuteAsync(statement.ElseBranch);
            }

            return;
        }

        private void Locate(Statement.LocateStatement statement)
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
            if (_textOutputDevice.AsyncAvailable)
            {
                _textOutputDevice.SetCursorPosition(y, x);
            }
        }

        private async Task Print(Statement.PrintStatement statement)
        {
            object value = Evaluate(statement.Expression);
            string crlf = statement.AddCrLf ? "\r\n" : "";

            if (_textOutputDevice.AsyncAvailable)
            {
                await _textOutputDevice.PrintAsync(value + crlf, _textOutputDevice.GetCursorPosition().Y, _textOutputDevice.GetCursorPosition().X);
            }
            else
            {
                _textOutputDevice.Print(value + crlf, _textOutputDevice.GetCursorPosition().Y, _textOutputDevice.GetCursorPosition().X);
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

            if (!(callee is ICallable))
            {
                throw new RuntimeException("You can only call functions.", expression.Paren);
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
