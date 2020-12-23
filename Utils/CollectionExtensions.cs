using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Utils
{
    public static class CollectionExtensions
    {
        public static IEnumerable<bool> CharToBool(this IEnumerable<char> source, Func<char, bool> isChar)
        {
            return source.Select(c => isChar(c));
        }

        public static IEnumerable<T> SkipEnds<T>(this IEnumerable<T> source)
        {
            var length = source.Count() - 2;
            if (length < 0)
                return source;
            return source.Skip(1).Take(length);
        }

        public static void AddKvp<T1, T2>(this Dictionary<T1, T2> dictionary, KeyValuePair<T1, T2> pair)
        {
            dictionary.Add(pair.Key, pair.Value);
        }

        public static void AddOrOverwrite<T1, T2>(this IDictionary<T1, T2> dictionary, T1 key, T2 value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return;
            }
            dictionary.Add(key, value);
        }

        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
            foreach (var sequence in sequences)
            {
                result =
                  from seq in result
                  from item in sequence
                  select seq.Concat(new[] { item });
            }
            return result;
        }

        public static void Rotate<T>(this List<List<T>> matrix)
        {
            T tmp = default;
            var n = matrix.Count;
            if (n != matrix.First().Count)
                throw new InvalidOperationException("Rotation algorithm will only work on square sets");

            for (int i = 0; i < n / 2; i++)
                for (int j = i; j < n - i - 1; j++)
                {
                    tmp = matrix[i][j];
                    matrix[i][j] = matrix[j][n - i - 1];
                    matrix[j][n - i - 1] = matrix[n - i - 1][n - j - 1];
                    matrix[n - i - 1][n - j - 1] = matrix[n - j - 1][i];
                    matrix[n - j - 1][i] = tmp;
                }
        }
    }
}