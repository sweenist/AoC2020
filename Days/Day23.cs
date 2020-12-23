using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day23
    {
        private static readonly string _sampleInput = "389125467";

        private static readonly string _input = "389547612";

        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            ShuffleCups(_sampleInput, 10).Should().Be("92658374");
            ShuffleCups(_sampleInput, 100).Should().Be("67384529");

            var result = ShuffleCups(_input, 100);
            Console.WriteLine($"Cups are {result}");
        }

        private static void Problem2()
        {

        }

        private static string ShuffleCups(string input, int rounds)
        {
            var cups = input.Select(c => Convert.ToInt32(c.ToString())).ToList();
            var cupCount = cups.Count;
            for (var i = 0; i < rounds; ++i)
            {
                var currentIndex = i % cupCount;
                var currentValue = cups[currentIndex];
                var takenCups = GetTakenCups(currentIndex).ToList();
                var destinationIndex = GetDestinationIndex(currentValue, takenCups);
                Shift(currentIndex, destinationIndex, takenCups);
            }

            var onesIndex = cups.IndexOf(1);
            var returnString = "";
            for (var i = 1; i < cupCount; ++i)
            {
                returnString += cups[(onesIndex + i) % cupCount];
            }

            return returnString;

            #region local methods

            IEnumerable<int> GetTakenCups(int currentIndex)
            {
                for (var x = 1; x <= 3; ++x)
                    yield return cups[(currentIndex + x) % cupCount];
            }

            int GetDestinationIndex(int currentValue, IEnumerable<int> missingCups)
            {
                var decrementor = 0;
                while (true)
                {
                    decrementor++;
                    var nextValue = (currentValue + cupCount - decrementor) % cupCount;
                    nextValue = nextValue == 0 ? cupCount : nextValue;

                    if (missingCups.Contains(nextValue))
                        continue;

                    return (cups.IndexOf(nextValue) + cupCount) % cupCount;
                }
            }

            void Shift(int currentIndex, int destinationIndex, List<int> missingCups)
            {
                var nextIndex = (currentIndex + 1) % cupCount;
                var shiftFromIndex = (nextIndex + 3);
                var wrapAround = destinationIndex < currentIndex;

                destinationIndex = wrapAround
                    ? destinationIndex += cupCount
                    : destinationIndex;
                
                if(destinationIndex - shiftFromIndex >= cupCount)
                    destinationIndex -= cupCount;

                while (shiftFromIndex <= destinationIndex)
                {
                    cups[nextIndex] = cups[shiftFromIndex % cupCount];

                    shiftFromIndex++;

                    nextIndex++;
                    nextIndex %= cupCount;
                }

                for (var x = 0; x < 3; ++x)
                {
                    cups[nextIndex] = missingCups[x];
                    nextIndex++;
                    nextIndex %= cupCount;
                }
            }

            #endregion
        }
    }
}