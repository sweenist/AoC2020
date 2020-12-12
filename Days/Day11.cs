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
            var result = NormalizeSeatOccupancy(seats.Seats);
            result.Should().Be(37);

            var seats2 = ParseSeats(_input);
            var result2 = NormalizeSeatOccupancy(seats2.Seats);
            Console.WriteLine($"Occupied seats: {result2}");
        }

        private static void Problem2()
        {
            var seatingArea = ParseSeats(_sampleInput);
            var result = NormalizeDirectionalSeatOccupancy(seatingArea);
            result.Should().Be(26);

            seatingArea = ParseSeats(_input);
            result = NormalizeDirectionalSeatOccupancy(seatingArea);
            Console.WriteLine($"Occupied seats: {result}");
        }

        private static void Test1()
        {
        }

        private static LoungeArea ParseSeats(IEnumerable<string> input)
        {
            var rowCount = input.Count();
            var rowWidth = input.First().Length;

            var seats = new Seat[rowCount, rowWidth];

            var row = 0;
            foreach (var tileRow in input.Select(line => line.CharToBool(c => c.Equals('L'))))
            {
                var _ = tileRow.Select((b, i) =>
                {
                    if (!b)
                        return default(Seat);

                    var seat = new Seat(row, i, rowWidth);
                    seats[row, i] = seat;
                    AddAdjacentSeat(seat, seats, row, i);
                    AddVisibleSeat(seat, seats, row, i);

                    return seats[row, i];
                }).ToList();

                row++;
            }
            return new LoungeArea(seats);
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

        private static void AddVisibleSeat(Seat seat, Seat[,] seats, int row, int column)
        {
            var rowWidth = seats.GetUpperBound(1);
            var directionVectors = new Dictionary<Direction, (int X, int Y)>
            {
                { Direction.NW, (X:-1, Y: -1)},
                { Direction.N,  (X: 0, Y: -1)},
                { Direction.NE, (X: 1, Y: -1)},
                { Direction.W,  (X:-1, Y:  0)},
            };

            foreach (var key in directionVectors.Keys)
            {
                var r = row;
                var c = column;
                while (true)
                {
                    c = c + directionVectors[key].X;
                    r = r + directionVectors[key].Y;

                    if (!c.IsInBounds(rowWidth) || r < 0)
                        break;

                    var targetSeat = seats[r, c];
                    if (targetSeat is null)
                        continue;

                    seat.VisibleSeats.Add(targetSeat);
                    targetSeat.VisibleSeats.Add(seat);
                    break;
                }
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

        private static int NormalizeDirectionalSeatOccupancy(LoungeArea area)
        {
            var isStillInFlux = true;
            var fluxCount = 0;

            while (isStillInFlux)
            {
                var preList = area.Seats.Select(s => s.IsOccupied).ToList();
                var seatsToChange = area.Seats.Where(
                                    s => (!s.IsOccupied && s.VisibleSeats.All(s => !s.IsOccupied))
                                        || (s.IsOccupied && s.VisibleSeats.Count(s => s.IsOccupied) >= 5)).ToList();

                seatsToChange.All(s => s.Swap());

                var postList = area.Seats.Select(s => s.IsOccupied).ToList();
                isStillInFlux = postList.Zip(preList, (first, second) => first == second).Any(r => !r);
                fluxCount++;

                // VisualizeSample(area);
            }

            Console.WriteLine($"{fluxCount} iterations to stasis");
            return area.Seats.Count(s => s.IsOccupied);
        }

        private static void VisualizeSample(LoungeArea area)
        {
            Console.WriteLine();

            var output = string.Empty;
            for (var i = 0; i < area.ColumnHeight; i++)
            {
                var temp = Enumerable.Repeat('.', area.RowWidth + 1).ToArray();

                var rowMin = (i * (area.ColumnHeight + 1));
                var sample = area.Seats.Where(s => s.Id.Includes(rowMin, rowMin + area.RowWidth))
                                       .Select(s => new { Index = s.Id - i * area.ColumnHeight, Symbol = s.IsOccupied ? '#' : 'L' });

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

        private enum Direction
        {
            NW,
            N,
            NE,
            E,
            SE,
            S,
            SW,
            W
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
            public List<Seat> VisibleSeats { get; } = new List<Seat>();

            public bool Swap()
            {
                IsOccupied = !IsOccupied;
                return true; //Hijacking Linq like a bad boy
            }
        }

        private record LoungeArea(Seat[,] SeatArray)
        {
            public int RowWidth => SeatArray.GetUpperBound(1);
            public int ColumnHeight => SeatArray.GetUpperBound(0);
            public List<Seat> Seats => SeatArray.Cast<Seat>().Where(s => s is not null).ToList();
        }
    }
}