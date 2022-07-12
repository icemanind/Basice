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
        private ITextOutput _textOutputDevice;
        private readonly Dictionary<string, object> _variables;
        private readonly Dictionary<string, ICallable> _stdLib;

        public Interpreter(List<Statement> statements, ITextOutput textOutputDevice)
        {
            _statements = statements;
            _textOutputDevice = textOutputDevice;
            _variables = new Dictionary<string, object>();
            _stdLib = new Dictionary<string, ICallable>();

            _stdLib.Add("LEFT$", new Stdlib.Left());
            _stdLib.Add("LEN", new Stdlib.Len());
            _stdLib.Add("MID$", new Stdlib.Mid());
            _stdLib.Add("RGB", new Stdlib.Rgb());
            _stdLib.Add("RIGHT$", new Stdlib.Right());
            _stdLib.Add("STR$", new Stdlib.Str());
            _stdLib.Add("VAL", new Stdlib.Val());
        }

        public async Task InterpretAsync()
        {
            foreach (var statement in _statements)
            {
                await ExecuteAsync(statement);
            }
        }

        private async Task ExecuteAsync(Statement statement)
        {
            switch (statement.GetType().Name)
            {
                case nameof(Statement.ClsStatement):
                    await ClsAsync();
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
            if (_textOutputDevice.AsyncAvailable)
            {
                await _textOutputDevice.PrintAsync(value.ToString(), _textOutputDevice.GetCursorPosition().Y, _textOutputDevice.GetCursorPosition().X);
            }
            else
            {
                _textOutputDevice.Print(value.ToString(), _textOutputDevice.GetCursorPosition().Y, _textOutputDevice.GetCursorPosition().X);
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

        private object EvaluateUnary(Expression.Unary expression)
        {
            object right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.Minus:
                    IsNumberOperand(expression.Operator, right);
                    return -(double)right;
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
                _variables.Add(expression.Name.Lexeme.ToUpper(), 0);
                return 0;
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
        #endregion
    }
}
