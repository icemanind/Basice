using System.Globalization;

namespace Basice.UI
{
    public static class ConsoleColor
    {
        public static int Black = FromRgb(0, 0, 0);
        public static int Blue = FromRgb(0, 0, 255);
        public static int Brown = FromRgb(150, 75, 0);
        public static int Cyan = FromRgb(0, 100, 100);
        public static int Gray = FromRgb(192, 192, 192);
        public static int Grey = FromRgb(192, 192, 192);
        public static int Green = FromRgb(0, 128, 0);
        public static int Lime = FromRgb(0, 255, 0);
        public static int Maroon = FromRgb(128, 0, 0);
        public static int Olive = FromRgb(128, 128, 0);
        public static int Orange = FromRgb(255, 165, 0);
        public static int Peach = FromRgb(255, 229, 180);
        public static int Pink = FromRgb(255, 192, 203);
        public static int Purple = FromRgb(160, 32, 240);
        public static int Red = FromRgb(255, 0, 0);
        public static int Silver = FromRgb(192, 192, 192);
        public static int Tan = FromRgb(210, 180, 140);
        public static int Violet = FromRgb(143, 0, 255);
        public static int White = FromRgb(255, 255, 255);
        public static int Yellow = FromRgb(255, 255, 0);

        public static int FromRgb(int red, int green, int blue)
        {
            string hex = $"{red:x2}{green:x2}{blue:x2}";
            return int.Parse(hex, NumberStyles.HexNumber);
        }
    }
}
