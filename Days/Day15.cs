using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace AdventOfCode.Days
{
    public static class Day15
    {
        private static List<long> GetSampleInput() => new long[] { 0, 3, 6 }.ToList();
        private static List<long> GetInput() => new[] { 1L, 2, 16, 19, 18, 0 }.ToList();
        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            var r = Algorithm1(GetSampleInput(), 2020);
            r[3].Should().Be(0);
            r[4].Should().Be(3);
            r[5].Should().Be(3);
            r[6].Should().Be(1);
            r[7].Should().Be(0);
            r[8].Should().Be(4);
            r[9].Should().Be(0);
            r[2019].Should().Be(436);
            Algorithm1(new[] { 1L, 3, 2 }.ToList(), 2020)[2019].Should().Be(1);
            Algorithm1(new[] { 2L, 1, 3 }.ToList(), 2020)[2019].Should().Be(10);
            Algorithm1(new[] { 1L, 2, 3 }.ToList(), 2020)[2019].Should().Be(27);
            Algorithm1(new[] { 2L, 3, 1 }.ToList(), 2020)[2019].Should().Be(78);
            Algorithm1(new[] { 3L, 2, 1 }.ToList(), 2020)[2019].Should().Be(438);
            Algorithm1(new[] { 3L, 1, 2 }.ToList(), 2020)[2019].Should().Be(1836);

            var result = Algorithm1(GetInput(), 2020);
            Console.WriteLine($"The position at 2020 is {result.Last()}.");
        }

        private static void Problem2()
        {
            var r = Algorithm2(GetSampleInput());
            r.Last().Should().Be(175594);
            Algorithm2(new[] { 1L, 3, 2 }.ToList()).Last().Should().Be(2578);
            Algorithm2(new[] { 2L, 1, 3 }.ToList()).Last().Should().Be(3544142);
            Algorithm2(new[] { 1L, 2, 3 }.ToList()).Last().Should().Be(261214);
            Algorithm2(new[] { 2L, 3, 1 }.ToList()).Last().Should().Be(6895259);
            Algorithm2(new[] { 3L, 2, 1 }.ToList()).Last().Should().Be(18);
            Algorithm2(new[] { 3L, 1, 2 }.ToList()).Last().Should().Be(362);

            
            var result = Algorithm2(GetInput());
            Console.WriteLine($"The position at 30000000 is {result.Last()}.");
        }

        private static List<long> Algorithm1(List<long> nums, long maxCount)
        {
            for (var i = nums.Count - 1; i < maxCount - 1; ++i)
            {
                var spaceBetweenRepeatOfLastNumber = nums.LastIndexOf(nums[i]) - nums.LastIndexOf(nums[i], i - 1);
                nums.Add(nums.Count(n => n == nums[i]) > 1 ? spaceBetweenRepeatOfLastNumber : 0);
            }
            return nums;
        }

        private static List<long> Algorithm2(List<long> nums)
        {
            var indexBucket = nums.Select((n, i) => new KeyValuePair<long, int>(n, i))
                                  .GroupBy(kvp => kvp.Key)
                                  .Select(g => new KeyValuePair<long, int>(g.Key, g.Max(k => k.Value)))
                                  .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            for (var i = nums.Count - 1; i < 30000000 - 1; ++i)
            {
                if (indexBucket.ContainsKey(nums[i]))
                {
                    nums.Add(i - indexBucket[nums[i]]);
                    indexBucket[nums[i]] = i;
                }
                else
                {
                    nums.Add(0);
                    indexBucket.Add(nums[i], i);
                }
            }
            
            return nums;
        }
    }
}