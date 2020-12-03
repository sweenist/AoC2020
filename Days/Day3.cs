using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day3
    {
        private static string[] _input = File.ReadAllLines(".\\Days\\Inputs\\Day3.txt");

        public static int Problem1(int sampleSize = 0)
        {
            var inputLength = _input.First().Length;
            var treeCount = 0;

            var rowCount = sampleSize > 0
                ? sampleSize
                : _input.Count();

            for (var i = 1; i < rowCount; i++)
            {
                var index = (3 * i) % inputLength;
                if (_input[i][index].Equals('#'))
                    treeCount++;
            }
            return treeCount;
        }

        public static long Algorithm(int rightShift, int rowSkip)
        {
            var inputLength = _input.First().Length;
            var treeCount = 0L;

            for (var i = rowSkip; i < _input.Count(); i += rowSkip)
            {
                var index = (rightShift * i / rowSkip) % inputLength;
                if (_input[i][index].Equals('#'))
                    treeCount++;
            }
            return treeCount;
        }

        public static long Problem2()
        {
            var slope1 =  Algorithm(1, 1);
            var slope2 =  Algorithm(3, 1);
            var slope3 =  Algorithm(5, 1);
            var slope4 =  Algorithm(7, 1);
            var slope5 =  Algorithm(1, 2);

            Console.WriteLine($"slope 1 Trees: {slope1}");
            Console.WriteLine($"slope 2 Trees: {slope2}");
            Console.WriteLine($"slope 3 Trees: {slope3}");
            Console.WriteLine($"slope 4 Trees: {slope4}");
            Console.WriteLine($"slope 5 Trees: {slope5}");

            return slope1 * slope2 * slope3 * slope4 * slope5;
        }

        public static void Test1()
        {
            var expectations = new Dictionary<int, int>{
                {6, 0},
                {7, 1},
                {9, 2},
                {10, 3},
                {12, 5},
                {13, 5},
                {14, 6},
            };

            Console.WriteLine("Beginning test...");
            Console.WriteLine();

            foreach (var key in expectations.Keys)
            {
                var result = Problem1(key);
                if (result != expectations[key])
                    Console.Write($"X");
                else
                    Console.Write(".");
            }
        }

        public static void Test2(int right, int down, int expectation)
        {
            _input = @"..#.#...#.#.#.##.....###.#....#
...........##.#...#.#..........
....#.....#..#.............#...
.#....###..##...#...##...#.#..#
#.......#.........#..#.......#.
...#.##..##...#.#......#.##.#..
#.#..##.....#.....#..##........
...#.####...#.##...#...........
.#...#..#..#....#.#.#.#.##.....
##.#..#.##..#......#..##.#.#..#
.#.##.....#.#...............#.#
..##.#.....#.....##..##.#....#.
#..#..........#...##........#..
#..##.#.#...............#..#...
..#....#...#.......#.......#...".Split(Environment.NewLine);

            Console.WriteLine("Beginning test...");
            Console.WriteLine($"There are {_input.Count()} lines");

            var result = Algorithm(right, down);
            if (result != expectation)
            {
                Console.WriteLine($"Failed. Expected {expectation} but got {result}.");
            }
            else
            {
                Console.WriteLine("Passed");

            }
        }
    }
}