using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Basice.UI
{
    public static class Extensions
    {
        public static string RemoveFirstLines(this string text, int linesCount)
        {
            var lines = Regex.Split(text, "\r\n|\r|\n").Skip(linesCount);
            return string.Join(Environment.NewLine, lines.ToArray());
        }
    }
}
