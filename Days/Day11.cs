using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day11
    {
        private static Type _classType = typeof(Day11);
        private static readonly IEnumerable<string> _sampleInput = LoadSample(_classType).ToLines();
        private static readonly IEnumerable<string> _input = LoadInput(_classType).ToLines();
        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            var seats = ParseSeats(_sampleInput);
            var result = NormalizeSeatOccupancy(seats);
            result.Should().Be(37);

            var seats2 = ParseSeats(_input);
            var result2 = NormalizeSeatOccupancy(seats2);
            Console.WriteLine($"Occupied seats: {result2}");
        }

        private static void Problem2()
        {
        }

        private static void Test1()
        {
        }

        private static List<Seat> ParseSeats(IEnumerable<string> input)
        {
            var rowCount = input.Count();
            var columnCount = input.First().Length;

            var seats = new Seat[rowCount, columnCount];

            var row = 0;
            foreach (var tileRow in input.Select(line => line.CharToBool(c => c.Equals('L'))))
            {
                var _ = tileRow.Select((b, i) =>
                {
                    if (!b)
                        return default(Seat);

                    var seat = new Seat(row, i, columnCount);
                    seats[row, i] = seat;
                    AddAdjacentSeat(seat, seats, row, i);

                    return seats[row, i];
                }).ToList();

                row++;
            }
            return seats.Cast<Seat>().Where(s => s is not null).ToList();
        }

        private static void AddAdjacentSeat(Seat seat, Seat[,] seats, int row, int column)
        {
            var rowWidth = seats.GetUpperBound(1);
            Seat adjacentSeat = null;
            if (row > 0)
                for (var x = -1; x <= 1; x++)
                    if ((column + x).IsInBounds(rowWidth) && seats[row - 1, column + x] is not null)
                    {
                        adjacentSeat = seats[row - 1, column + x];
                        seat.AdjacentSeats.Add(adjacentSeat);
                        adjacentSeat.AdjacentSeats.Add(seat);
                    }
            if (column > 0 && seats[row, column - 1] is not null)
            {
                adjacentSeat = seats[row, column - 1];
                seat.AdjacentSeats.Add(adjacentSeat);
                adjacentSeat.AdjacentSeats.Add(seat);
            }
        }

        private static int NormalizeSeatOccupancy(List<Seat> seats)
        {
            var isStillInFlux = true;
            var fluxCount = 0;

            while (isStillInFlux)
            {
                var preList = seats.Select(s => s.IsOccupied).ToList();
                var seatsToChange = seats.Where(
                    s => (!s.IsOccupied && s.AdjacentSeats.All(s => !s.IsOccupied))
                        || (s.IsOccupied && s.AdjacentSeats.Count(s => s.IsOccupied) >= 4)).ToList();

                seatsToChange.All(s => s.Swap());

                var postList = seats.Select(s => s.IsOccupied).ToList();
                isStillInFlux = postList.Zip(preList, (first, second) => first == second).Any(r => !r);
                fluxCount++;

                // VisualizeSample(seats);
            }


            Console.WriteLine($"{fluxCount} iterations to stasis");
            return seats.Count(s => s.IsOccupied);
        }

        private static void VisualizeSample(List<Seat> seats)
        {
            Console.WriteLine();
            var rowCount = 99;
            var columnCount = 90;

            var output = string.Empty;
            for (var i = 0; i < rowCount; i++)
            {
                var temp = Enumerable.Repeat('.', columnCount).ToArray();

                var sample = seats.Where(s => s.Id.Includes(i * rowCount, i * rowCount + columnCount))
                                  .Select(s => new { Index = s.Id - i * rowCount, Symbol = s.IsOccupied ? '#' : 'L' });

                foreach (var blah in sample)
                {
                    temp[blah.Index] = blah.Symbol;
                }
                output += string.Concat(temp);
                output += Environment.NewLine;

            }
            Console.WriteLine(output);
            Console.WriteLine();
            Console.WriteLine($"{output.Count(c => c == '#')} Occupied seats");
        }

        private class Seat
        {
            public Seat(int row, int column, int columnCount)
            {
                Id = row * columnCount + column;
            }

            public int Id { get; }
            public bool IsOccupied { get; private set; }

            public List<Seat> AdjacentSeats { get; set; } = new List<Seat>();

            public bool Swap()
            {
                IsOccupied = !IsOccupied;
                return true; //Hijacking Linq like a bad boy
            }
        }
    }
}