namespace Basice.Interpreter.Interpreter
{
    public class CursorLocation
    {
        public int X { get; set; }
        public int Y { get; set; }

        public CursorLocation()
        {
            X = 0;
            Y = 0;
        }

        public CursorLocation(int y, int x)
        {
            X = x;
            Y = y;
        }
    }
}
