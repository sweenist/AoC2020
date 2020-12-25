using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day24
    {
        private static Type _classType = typeof(Day24);

        private static readonly List<string> _sampleInput = LoadSample(_classType).ToLines(StringSplitOptions.TrimEntries).ToList();
        private static readonly List<string> _input = LoadInput(_classType).ToLines(StringSplitOptions.TrimEntries).ToList();

        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
        }

        private static void Problem2()
        {
        }
    }
}