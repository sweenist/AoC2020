using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day13
    {
        private static Type _classType = typeof(Day13);
        private static readonly string[] _sampleInput = LoadSample(_classType).ToLines();
        private static readonly string[] _input = LoadInput(_classType).ToLines();
        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            var result = Algorithm1(_sampleInput);
            result.Should().Be(295);

            Console.WriteLine($"busId * waitTime is {Algorithm1(_input)}");
        }

        private static void Problem2()
        {
            ElapsedAction(() => Algorithm2(_sampleInput[1])).Should().Be(1068781);
            // Algorithm2("67,7,59,61").Should().Be(754018);
            // Algorithm2("17,x,13,19").Should().Be(3417);

            var result = ElapsedAction(() => Algorithm2(_input[1], 591401030L));
            Console.WriteLine($"Consecutive busses at {result}");

        }

        private static int Algorithm1(string[] lines)
        {
            int GetWaitTime(int current, int busId) => busId - (current % busId);
            if (lines.Length > 2)
                throw new ArgumentException("Only two strings allowed in array.");
            var currentTime = Convert.ToInt32(lines[0]);
            var busIds = lines[1].Split(',')
                                 .Where(v => v != "x")
                                 .Select(v => Convert.ToInt32(v))
                                 .Select(id => new { Id = id, WaitTime = GetWaitTime(currentTime, id) })
                                 .OrderBy(o => o.WaitTime);

            return busIds.Select(a => a.Id * a.WaitTime).First();
        }

        private static long Algorithm2(string line, long seed = 0L)
        {
            long GetIncrementBaseValue(IEnumerable<BusSchedule> scheds)
            {
                var initialValue = scheds.First().Id;
                var valueAtOffset = scheds.Single(s => s.Offset == initialValue).Id;
                return (long)(initialValue * valueAtOffset);
            }

            long Increment(long baseValue, long iter, int firstValue) => (long)(baseValue * iter - firstValue);
            var scheduled = line.Split(',')
                                .Select((v, i) => new BusSchedule(v, i))
                                .Where(b => b.Id != -1)
                                .ToList();

            var incrementor = GetIncrementBaseValue(scheduled);
            var iteration = seed;
            var initialValue = scheduled.First().Id;

            while (true)
            {
                var timestamp = Increment(incrementor, iteration, initialValue);
                if (scheduled.All(s => s.MatchesOffset(timestamp)))
                    return timestamp;

                if (timestamp % 100000000 == 0)
                    Console.WriteLine($"Timespan is {timestamp}. No matches yet.");
                
                iteration++;
            }

        }

        private class BusSchedule
        {
            public BusSchedule(string id, int offset)
            {
                Id = Int32.TryParse(id, out var busId)
                    ? busId
                    : -1;
                Offset = offset;
            }

            public int Id { get; }
            public int Offset { get; }

            public bool MatchesOffset(long time) => (time + Offset) % Id == 0L;
        }
    }
}