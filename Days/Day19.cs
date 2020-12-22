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
        private static readonly string[] _sample2 = @"42: 9 14 | 10 1
9: 14 27 | 1 26
10: 23 14 | 28 1
1: ""a""
11: 42 31
5: 1 14 | 15 1
19: 14 1 | 14 14
12: 24 14 | 19 1
16: 15 1 | 14 14
31: 14 17 | 1 13
6: 14 14 | 1 14
2: 1 24 | 14 4
0: 8 11
13: 14 3 | 1 12
15: 1 | 14
17: 14 2 | 1 7
23: 25 1 | 22 14
28: 16 1
4: 1 1
20: 14 14 | 1 15
3: 5 14 | 16 1
27: 1 6 | 14 18
14: ""b""
21: 14 1 | 1 14
25: 1 1 | 1 14
22: 14 14
8: 42
26: 14 22 | 1 20
18: 15 15
7: 14 5 | 1 21
24: 14 1

abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa
bbabbbbaabaabba
babbbbaabbbbbabbbbbbaabaaabaaa
aaabbbbbbaaaabaababaabababbabaaabbababababaaa
bbbbbbbaaaabbbbaaabbabaaa
bbbababbbbaaaaaaaabbababaaababaabab
ababaaaaaabaaab
ababaaaaabbbaba
baabbaaaabbaaaababbaababb
abbbbabbbbaaaababbbbbbaaaababb
aaaaabbaabaaaaababaa
aaaabbaaaabbaaa
aaaabbaabbaaaaaaabbbabbbaaabbaabaaa
babaaabbbaaabaababbaabababaaab
aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba".ToLines();

        private static readonly string[] _input = LoadInput(_classType).ToLines();

        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            // var r = Parse(_sampleInput);
            // r.Should().Be(2);

            // var result = Parse(_input);
            // Console.WriteLine($"Matches rule 0: {result}");
        }

        private static void Problem2()
        {
            // var a = Parse(_sample2);
            // Console.WriteLine(a);
            var r = Parse(_sample2);
            r.Should().Be(12);

            var result = Parse(_input);
            Console.WriteLine($"Matches exoanded rules: {result}");
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

                        // Think I still can't do yield return from LINQ methods
                        foreach (var solved in solvedProduct)
                        {
                            yield return string.Join(string.Empty, solved);
                        }
                    }
                }
            }

            var count_42 = new HashSet<string>(Solve(42));
            var count_31 = new HashSet<string>(Solve(31));
            var output = new HashSet<string>(Solve(0));

            var rule_4231Length = count_42.Concat(count_31).Select(s => s.Length).Distinct().Single();
            var baseLength = output.First().Length;

            var count = 0;
            foreach (var d in data)
            {
                if (d.Length == baseLength && output.Contains(d))
                    count++;
                if (d.Length > baseLength && d.Length % rule_4231Length == 0)
                {
                    var sections = d.SplitEvery(rule_4231Length);
                    var frontLoad = sections.TakeWhile(s => count_42.Contains(s));
                    if (frontLoad.Count() <= 2)
                        continue;
                    var front = string.Join(string.Empty, sections.TakeWhile(s => count_42.Contains(s)));
                    var back = string.Join(string.Empty, sections.Reverse().TakeWhile(s => count_31.Contains(s)).Reverse());

                    if (front == string.Empty || back == string.Empty)
                        continue;
                    if (d == front + back)
                        count++;
                }
            }
            return count;
        }
    }
}