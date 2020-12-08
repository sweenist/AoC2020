using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Utils;
using FluentAssertions;

namespace AdventOfCode.Days
{
    public class Day6
    {
        private static string _sampleInput = @"abc

a
b
c

ab
ac

a
a
a
a

b";

        private static string _input = Extensions.LoadInput(typeof(Day6));
        public static int Problem1()
        {
            var result = Process(_sampleInput);
            result.Should().Be(11);

            result = Process(_input);

            return result;
        }

        public static int Problem2()
        {
            var result = Process2(_sampleInput);
            result.Should().Be(6);

            result = Process2(_input);

            return result;
        }

        private static int Process(string input)
        {
            var groups = input.Split(Environment.NewLine.Repeat(2));
            return groups.Select(g => g.Replace(Environment.NewLine, string.Empty))
                  .Select(s => new HashSet<char>(s.GetEnumerator().Enumerate()))
                  .Sum(h => h.Count());
        }

        private static int Process2(string input)
        {
            var result = 0;
            foreach(var group in input.Split(Environment.NewLine.Repeat(2)))
            {
                var responses = group.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                result += string.Concat(responses)
                                .GroupBy(c => c)
                                .Where(c => c.Count().Equals(responses.Length))
                                .Count();
            }
                  
            return result;
        }
    }
}