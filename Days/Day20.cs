using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day20
    {
        private static Type _classType = typeof(Day20);
        private static readonly string[] _sampleInput = LoadSample(_classType).ToLines();

        private static readonly string[] _input = LoadInput(_classType).ToLines();

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

        private static List<Tile> ParseTiles(string[] input)
        {
            return default;
        }

        private class Tile
        {
            public Tile(int id, IEnumerable<String> tileSchema)
            {
                Id = id;
                TileSchema = tileSchema.Select(line => line.Select(s => s).ToList()).ToList();
            }

            public int Id { get; set; }

            public List<List<char>> TileSchema { get; set; }

            public void Rotate() { }
            public void Flip() { }
        }
    }
}