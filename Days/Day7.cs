using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day7
    {
        private static string _bagColor = "shiny gold";
        private static Type _classType = typeof(Day7);
        public static void Run()
        {
            // Problem1();
        }

        private static void Problem1()
        {
            var sampleResult = ProcessBags(LoadSample(_classType)).ProcessInteriorBags()
                                                                  .FindContainers(_bagColor)
                                                                  .Count();
            Console.WriteLine($"There are {sampleResult} unique bags that can hold {_bagColor}");

            var result = ProcessBags(LoadInput(_classType)).ProcessInteriorBags()
                                                           .FindContainers(_bagColor)
                                                           .Count();

            Console.WriteLine($"There are {result} unique bags that can hold {_bagColor}");
        }

        private static IEnumerable<Luggage> ProcessBags(string input)
        {
            (string, string[]) ParseLine(string line)
            {
                var color = line.Substring(0, line.IndexOf(" bags contain"));
                var includedBags = line.Contains("no other bags")
                    ? Enumerable.Empty<string>().ToArray()
                    : line.Substring(line.IndexOf("contain ") + 8).Split(',');
                return (color, includedBags);
            }

            return input.ToLines()
                        .Select(s => ParseLine(s))
                        .Select(s => new Luggage(s.Item1, s.Item2));
        }

        private static IEnumerable<Luggage> ProcessInteriorBags(this IEnumerable<Luggage> source)
        {
            var luggageList = source.ToList();
            foreach (var bag in source)
            {
                bag.ProcessLuggage(luggageList);
                yield return bag;
            }
        }

        private static IEnumerable<string> FindContainers(this IEnumerable<Luggage> source, string bagColor)
        {
            var collection = source.Where(s => s.ContainedBags.ContainsKey(bagColor))
                                   .Select(b => b.Color)
                                   .ToHashSet();

            foreach (var container in collection.Select(c => source.FindContainers(c)).ToList())
            {
                collection.UnionWith(container);
            }

            return collection;
        }

        private class Luggage
        {
            private readonly string[] _containingBags;
            private Regex _pattern = new Regex(@"(?<quantity>\d+)\s(?<color>.*)\sbag", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            public Luggage(string color, params string[] containingBags)
            {
                Color = color;
                _containingBags = containingBags;
            }

            public string Color { get; }
            public Dictionary<string, int> ContainedBags { get; } = new Dictionary<string, int>();

            public bool Contains(string color) => ContainedBags.ContainsKey(color);

            public void ProcessLuggage(IList<Luggage> allBags)
            {
                foreach (var bag in _containingBags)
                {
                    var match = _pattern.Match(bag);
                    if (match.Success)
                    {
                        var luggage = allBags.Single(b => b.Color.Equals(match.Groups["color"].Value));
                        ContainedBags.Add(luggage.Color, Convert.ToInt32(match.Groups["quantity"].Value));
                    }
                }
            }
        }
    }
}