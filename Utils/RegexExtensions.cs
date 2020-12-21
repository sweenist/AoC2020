using System;
using System.Text.RegularExpressions;

namespace AdventOfCode.Utils
{
    public static class RegexExtensions
    {
        public static string Value(this GroupCollection group, string key)
        {
            return group[key].Value;
        }

        public static int Int32Value(this GroupCollection group, string key)
        {
            return Convert.ToInt32(group[key].Value);
        }

        public static bool Matches(this string value, string pattern)
        {
            var r = new Regex(pattern);
            return r.Match(value).Success;
        }
    }
}