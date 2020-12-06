using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Utils
{
    public static class Extensions
    {
        public static KeyValuePair<string, string> ToKeyValuePair(this string source, char delimiter)
        {
            var splitString = source.Split(delimiter, 2, System.StringSplitOptions.TrimEntries);
            return new KeyValuePair<string, string>(splitString[0], splitString[1]);
        }

        public static bool Includes(this int? value, int min, int max)
        {
            return value.HasValue && value.Value.Includes(min, max);
        }

        public static bool Includes(this int value, int min, int max)
        {
            return min <= value && value <= max;
        }

        public static string Repeat(this string value, int repeatCount)
        {
            return String.Concat(Enumerable.Repeat(value, repeatCount));
        }

        public static string Repeat(this char value, int repeatCount)
        {
            return String.Concat(Enumerable.Repeat(value, repeatCount));
        }

        public static IEnumerable<T> Enumerate<T>(this IEnumerator<T> source)
        {
            while(source.MoveNext())
            {
                yield return source.Current;
            }
        }
    }
}