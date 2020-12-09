using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day9
    {
        private static Type _classType = typeof(Day9);
        private static IEnumerable<int> _sampleInput = LoadSample(_classType).ToLines().Select(s => Convert.ToInt32(s));
        private static IEnumerable<int> _input = LoadInput(_classType).ToLines().Select(s => Convert.ToInt32(s));
        public static void Run()
        {
            Tests();
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            var sampleResult = ProcessPart1(_sampleInput, 5);
            sampleResult.Should().Be(127);

            var result = ProcessPart1(_input, 25);
            Console.WriteLine($"Unsummable with given set: {result}");
        }

        private static void Problem2()
        {
            
            var target = 23278925;
        }

        private static void Tests()
        {

            var set = Enumerable.Range(1, 25);

            set.First().Should().Be(1);
            set.Last().Should().Be(25);

            set.Skip(2).Take(3).Should().ContainInOrder(new[] { 3, 4, 5 });
            set.Skip(5).First().Should().Be(6);
        }

        private static int ProcessPart1(IEnumerable<int> input, int take)
        {
            var skip = 0;
            var searching = true;
            while (searching)
            {
                var currentSet = input.Skip(skip).Take(take);
                var targetSum = input.Skip(skip + take).First();
                var found = false;

                while (!found)
                {
                    skip++;
                    for (var i = 0; i < take - 1; i++)
                    {
                        for (var j = i + 1; j < take; j++)
                        {
                            found = currentSet.ElementAt(i) + currentSet.ElementAt(j) == targetSum;
                            if (found)
                                break;
                        }
                        if (found)
                            break;
                    }
                    if (!found)
                        return targetSum;
                }
            }
            throw new IndexOutOfRangeException($"Couldn't find a target sum that did not equal any two numbers in a sample of the previous 5");
        }
    }
}