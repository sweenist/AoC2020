using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day19
    {
        private static Type _classType = typeof(Day19);
        private static readonly string[] _sampleInput = LoadSample(_classType).ToLines();

        private static readonly string[] _input = LoadInput(_classType).ToLines();

        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            var r = Parse(_sampleInput);
            r.Should().Be(2);

            var result = Parse(_input);
            Console.WriteLine($"Matches rule 0: {result}");
        }

        private static void Problem2()
        {
        }

        private static int Parse(string[] input)
        {
            var rules = input.Where(line => line.IndexOf(':') > -1)
                             .Select(line => new KeyValuePair<int, string>(
                                            line.Split(':', StringSplitOptions.TrimEntries)[0].ToInt32().Single(),
                                            line.Split(':', StringSplitOptions.TrimEntries)[1]))
                             .OrderBy(kvp => kvp.Key)
                             .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var data = input.Where(line => line.StartsWith('a') || line.StartsWith('b'))
                            .ToList();

            IEnumerable<string> Solve(int index)
            {
                var rule = rules[index];
                if (rule.StartsWith('"'))
                    yield return rule.Substring(1, 1);
                else
                {
                    foreach (var sides in rule.Split('|', StringSplitOptions.TrimEntries))
                    {
                        var solvedProduct = sides.Split(' ', StringSplitOptions.TrimEntries)
                                                 .Select(s => Convert.ToInt32(s))
                                                 .Select(i => Solve(i))
                                                 .CartesianProduct();

                        // Think I still can';'t do yield return from LINQ methods
                        foreach (var solved in solvedProduct)
                        {
                            yield return string.Join(string.Empty, solved);
                        }
                    }
                }
            }

            var output = new HashSet<string>(Solve(0));
            return output.Where(s => data.Contains(s)).Count();
        }
    }
}