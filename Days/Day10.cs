using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day10
    {
        private static Type _classType = typeof(Day10);
        private static readonly IEnumerable<int> _sampleInput = LoadSample(_classType).ToInt32();
        private static readonly IEnumerable<int> _input = LoadInput(_classType).ToInt32();
        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            Test1();
            var result = ProcessAdapters(_input);
            Console.WriteLine($"Adapter product of 1 and 3 differences: {result}");
        }

        private static void Problem2()
        {
            var result = ProcessTree(_sampleInput);
            result.Should().Be(19208);

            result = ProcessTree(_input);
            Console.WriteLine($"There are {result} unique adapter paths.");
        }

        private static void Test1()
        {
            var result = ProcessAdapters(_sampleInput);
            result.Should().Be(220);
        }

        private static int ProcessAdapters(IEnumerable<int> input)
        {
            var diff1 = 0;
            var diff3 = 0;
            var set = input.Append(input.Max() + 3).OrderBy(_ => _)
                           .Aggregate(0, (res, inc) =>
                           {
                               var diff = inc - res;
                               if (diff == 1)
                                   diff1++;
                               if (diff == 3)
                                   diff3++;
                               return inc;
                           });

            return diff1 * diff3;
        }


        private static long ProcessTree(IEnumerable<int> input)
        {
            var set = input.Prepend(0)
                           .OrderBy(_ => _)
                           .ToList();

            var pathways = new long[set.Count];
            pathways[0] = 1; //in any given set of paths, there is at least 1 path

            for (var i = 0; i < set.Count; i++)
            {
                var current = set[i];
                var currentPathway = pathways[i];

                for (var j = 1; j <= 3; j++)
                {
                    var index = i + j;
                    if (index >= set.Count || set[index] > current + 3)
                        break;

                    pathways[index] = pathways[index] + currentPathway;
                }
            }

            return pathways.Last();
        }
    }
}