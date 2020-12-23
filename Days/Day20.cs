using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day20
    {
        private static Type _classType = typeof(Day20);
        private static readonly string[] _sampleInput = LoadSample(_classType).Trim().ToLines(StringSplitOptions.TrimEntries);

        private static readonly string[] _input = LoadInput(_classType).Trim().ToLines(StringSplitOptions.TrimEntries);

        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            var sampleTiles = ParseTiles(_sampleInput);
            MatchBorders(sampleTiles);
            SolveCorners(sampleTiles).Should().Be(20899048083289);

            var tiles = ParseTiles(_input);
            MatchBorders(tiles);
            var result = SolveCorners(tiles);

            Console.WriteLine($"Corner math is {result}");
        }

        private static void Problem2()
        {
            var sampleTiles = ParseTiles(_sampleInput);
            MatchBorders(sampleTiles);
            sampleTiles.Count(c => c.IsEdge).Should().Be(4);
            sampleTiles.First().Rotate();
            OrientTiles(sampleTiles);

            var tiles = ParseTiles(_input);
            MatchBorders(tiles);
            tiles.Count(t => t.IsEdge).Should().Be(40);
            tiles.First().Rotate();

        }

        private static long SolveCorners(List<Tile> tiles)
        {
            return tiles.Where(t => t.IsCorner).Aggregate(1L, (f, s) => f * s.Id);
        }

        private static void MatchBorders(List<Tile> tiles)
        {
            for (var i = 0; i < tiles.Count; ++i)
                for (var j = i + 1; j < tiles.Count; ++j)
                    tiles[i].Match(tiles[j]);
        }

        private static void OrientTiles(List<Tile> tiles)
        {
            tiles.ForEach(x => x.OrientEdges());
            var size = (int)Math.Sqrt(tiles.Count);
            var tileArray = new Tile[size, size];
            tileArray[0, 0] = tiles.Single(t => t.NeighboringEdges == (Positions.Right | Positions.Bottom));

            for (var y = 0; y < size; ++y)
                for (var x = 0; x < size; ++x)
                {
                    if (x + y == 0)
                        continue;
                    if (y == 0)
                    {
                        var leftTile = tileArray[x - 1, y];
                        var thisTile = tiles.Where(t => (t.IsEdge || t.IsCorner && x == size - 1) 
                                                       && t.MatchedEdges.Any(m => m.Key == leftTile.Id));
                        // thisTile.Arrange(leftTile);
                    }
                }
        }

        private static List<Tile> ParseTiles(string[] input)
        {
            var tileRegex = new Regex(@"Tile (?<id>\d+):");
            var index = 0;
            var returnValue = new List<Tile>();
            while (index < input.Length)
            {
                // Id
                var id = Convert.ToInt32(tileRegex.Match(input[index]).Groups["id"].Value);
                index++;

                // Picture Data
                var tileInfo = input.Skip(index)
                                    .TakeWhile(s =>
                                    {
                                        index++;
                                        return !string.IsNullOrWhiteSpace(s);
                                    }).ToList();

                returnValue.Add(new Tile(id, tileInfo));
            }

            return returnValue;
        }

        [Flags]
        private enum Positions
        {
            None = 0,
            Top = 1 << 0,
            Right = 1 << 1,
            Bottom = 1 << 2,
            Left = 1 << 3
        }

        private class Tile
        {
            public Tile(int id, IEnumerable<String> tileSchema)
            {
                Id = id;
                TileSchema = tileSchema.Select(line => line.Select(s => s).ToList()).ToList();
                BuildEdges();
            }

            public int Id { get; set; }

            public List<List<char>> TileSchema { get; set; }

            public List<string> Edges { get; private set; }

            public HashSet<KeyValuePair<int, string>> MatchedEdges { get; } = new HashSet<KeyValuePair<int, string>>();

            public Positions NeighboringEdges { get; private set; }

            public bool IsCorner => MatchedEdges.Count().Equals(4);
            public bool IsEdge => MatchedEdges.Count().Equals(6);
            public void Rotate()
            {
                TileSchema.Rotate();
                BuildEdges();
            }

            public void Flip()
            {
                TileSchema.Reverse();
                BuildEdges();
            }

            public void Match(Tile other)
            {
                Edges.Where(m => other.Edges.Any(n => n == m))
                     .ToList()
                     .ForEach(x =>
                     {
                         MatchedEdges.Add(new KeyValuePair<int, string>(other.Id, x));
                         other.MatchedEdges.Add(new KeyValuePair<int, string>(Id, x));
                     });
            }

            public void OrientEdges()
            {
                NeighboringEdges = (Positions)MatchedEdges.Select(kvp => new { Index = 1 << Edges.IndexOf(kvp.Value) })
                                                          .Where(a => a.Index < 1 << 4)
                                                          .Aggregate(0, (a, b) => a + b.Index);
            }

            public void Arrange(Tile left, Tile top = null)
            {
                var leftEdge = left.Edges[GetIndex(Positions.Right)];
                var topEdge = top?.Edges[GetIndex(Positions.Bottom)];
                bool matched() => leftEdge == Edges[GetIndex(Positions.Left)] && (topEdge is null || topEdge == Edges[GetIndex(Positions.Top)]);

                while (!matched())
                {
                    if (Edges.IndexOf(leftEdge) > 3)
                        Flip();
                    if (leftEdge == Edges[GetIndex(Positions.Left)])
                        continue;
                    Rotate();
                }
            }

            private void BuildEdges()
            {
                /*
                ^---->
                |    |
                |    |
                |    |
                <----v
                */
                Edges = new List<string>();

                Edges.Add(new string(TileSchema.First().ToArray())); //Top
                Edges.Add(new string(TileSchema.Select(row => row.Last()).ToArray())); //Right
                Edges.Add(new string(TileSchema.Last().Reverse<char>().ToArray())); //Bottom
                Edges.Add(new string(TileSchema.Select(row => row.First()).Reverse<char>().ToArray())); //Left

                //Flip
                var flippedEdges = Edges.Select(s => new string(s.Reverse().ToArray())).ToList();
                Edges.Add(flippedEdges[2]); // Former Bottom; now top
                Edges.Add(flippedEdges[1]);
                Edges.Add(flippedEdges[0]); // Former Top, now bottom
                Edges.Add(flippedEdges[3]);
            }
        }


        private static int GetIndex(Positions flagsEnum)
        {
            return (int)(Math.Sqrt((int)flagsEnum));
        }
    }
}