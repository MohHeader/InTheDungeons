using System;

namespace Assets.Game.Scripts.Helpers
{
    public static class StringHelpers
    {
        public static string CutString(this string source, string prefix, string postfix)
        {
            var leftCut = source.Substring(source.IndexOf(prefix, StringComparison.OrdinalIgnoreCase) + prefix.Length);
            var rightCut = leftCut.Substring(0, leftCut.IndexOf(postfix, StringComparison.OrdinalIgnoreCase));
            return rightCut;
        }
    }
}