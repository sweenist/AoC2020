using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public static bool IsInBounds(this int value, int upperBound)
        {
            return 0 <= value && value <= upperBound;
        }

        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return !source.All(predicate);
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
            while (source.MoveNext())
            {
                yield return source.Current;
            }
        }

        public static string LoadInput(Type dayClass)
        {
            var className = dayClass.Name;
            return File.ReadAllText($"Days/Inputs/{className}.txt");
        }

        public static string LoadSample(Type dayClass)
        {
            var className = dayClass.Name;
            return File.ReadAllText($"Days/Samples/{className}.txt");
        }

        public static string[] ToLines(this string source)
        {
            if (source.Contains(Environment.NewLine))
                return source.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return source.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

        public static long[] ToInt64(this string source)
        {
            if (source.Contains(Environment.NewLine))
                return source.Split(Environment.NewLine).Select(s => Convert.ToInt64(s)).ToArray();
            return source.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt64(s)).ToArray();
        }

        public static int[] ToInt32(this string source)
        {
            if (source.Contains(Environment.NewLine))
                return source.Split(Environment.NewLine).Select(s => Convert.ToInt32(s)).ToArray();
            return source.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt32(s)).ToArray();
        }

        public static IEnumerable<bool> CharToBool(this IEnumerable<char> source, Func<char, bool> isChar)
        {
            return source.Select(c => isChar(c));
        }

        public static T ElapsedAction<T>(Func<T> action, string actionString = "Action")
        {
            var stopWatch = Stopwatch.StartNew();
            var result = action();
            stopWatch.Stop();
            Console.WriteLine($"{actionString} took {stopWatch.Elapsed.ToString(@"hh\:mm\:ss\.fff")}");
            return result;
        }
    }
}