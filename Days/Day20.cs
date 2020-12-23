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
            MatchEdges(sampleTiles);
            SolveCorners(sampleTiles).Should().Be(20899048083289);

            var tiles = ParseTiles(_input);
            MatchEdges(tiles);
            var result = SolveCorners(tiles);

            Console.WriteLine($"Corner math is {result}");
        }

        private static void Problem2()
        {

        }

        private static long SolveCorners(List<Tile> tiles)
        {
            var cornerTilesMaybe = tiles.Where(t => t.MatchedEdges.Count == 4);
            return tiles.Where(t => t.MatchedEdges.Count == 4).Aggregate(1L, (f, s) => f * s.Id);
        }

        private static void MatchEdges(List<Tile> tiles)
        {
            for (var i = 0; i < tiles.Count; ++i)
                for (var j = i + 1; j < tiles.Count; ++j)
                    tiles[i].Match(tiles[j]);
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

            public List<KeyValuePair<int, string>> MatchedEdges { get; } = new List<KeyValuePair<int, string>>();

            public void Rotate() { }
            public void Flip() { }

            public void Match(Tile other)
            {
                Edges.Where(m => other.Edges.Any(n => n == m)).ToList().ForEach(x =>
                {
                    MatchedEdges.Add(new KeyValuePair<int, string>(other.Id, x));
                    other.MatchedEdges.Add(new KeyValuePair<int, string>(Id, x));
                });
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

                Edges.Add(new string(TileSchema.First().ToArray()));
                Edges.Add(new string(TileSchema.Select(row => row.Last()).ToArray()));
                Edges.Add(new string(TileSchema.Last().Reverse<char>().ToArray()));
                Edges.Add(new string(TileSchema.Select(row => row.First()).Reverse<char>().ToArray()));

                //Flip
                Edges = Edges.Concat(Edges.Select(s => new string(s.Reverse().ToArray()))).ToList();
            }
        }
    }
}