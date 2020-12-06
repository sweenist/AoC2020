using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using static AdventOfCode.Utils.BinaryExtensions;

namespace AdventOfCode.Days
{
    public class Day5
    {
        private static string[] _sampleInput = @"BFFFBBFRRR
FFFBBBFRRR
BBFFBBFRLL".Split(Environment.NewLine);

        private static string[] _input = File.ReadAllLines(@"Days\Inputs\Day5.txt");

        public static int Problem1(bool test)
        {
            var passes = Process(test ? _sampleInput : _input).ToList();
            if (test)
            {
                passes.Should().HaveCount(3)
                      .And.SatisfyRespectively(
                          first =>
                          {
                              first.Row.Should().Be(70);
                              first.Seat.Should().Be(7);
                              first.SeatId.Should().Be(567);
                          },
                          second =>
                          {
                              second.Row.Should().Be(14);
                              second.Seat.Should().Be(7);
                              second.SeatId.Should().Be(119);
                          },
                          last =>
                          {
                              last.Row.Should().Be(102);
                              last.Seat.Should().Be(4);
                              last.SeatId.Should().Be(820);
                          }
                      );
            }

            return passes.Max(p => p.SeatId);
        }

        public static int Problem2()
        {
            var passes = Process(_input).OrderBy(p => p.SeatId).Select(p => p.SeatId).ToList();
            var subCount = passes.Count - 1;
            var results = passes.Take(subCount)
                                .Zip(passes.TakeLast(subCount), (first, last) => new { First = first, Last = last, Diff = last - first })
                                .Single(a => a.Diff.Equals(2));
            
            return results.First + 1;
        }

        private static IEnumerable<BoardingPass> Process(string[] input)
        {
            foreach (var line in input)
            {
                yield return new BoardingPass(line);
            }
        }

        private record BoardingPass
        {
            public BoardingPass(string rawData)
            {
                Row = rawData.Substring(0, 7).ToBinaryCollection('B').ToInt32();
                Seat = rawData.Substring(7, 3).ToBinaryCollection('R').ToInt32();
            }

            public int Row { get; }
            public int Seat { get; }
            public int SeatId => (Row * 8) + Seat;
        }
    }
}