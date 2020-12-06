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

        private static string _input = File.ReadAllText(@"Days\Inputs\Day6.txt");
        public static int Problem1()
        {
            var result = Process(_sampleInput);
            result.Should().Be(11);

            result = Process(_input);

            return result;
        }

        private static int Process(string input)
        {
            var groups = input.Split(Environment.NewLine.Repeat(2));
            Console.WriteLine($"There are {groups.Count()} groups.");
            return groups.Select(g => g.Replace(Environment.NewLine, string.Empty))
                  .Select(s => new HashSet<char>(s.GetEnumerator().Enumerate()))
                  .Sum(h => h.Count());
        }
    }
}