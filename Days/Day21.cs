using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day21
    {
        private static Type _classType = typeof(Day21);
        private static readonly string[] _sampleInput = LoadSample(_classType).ToLines();

        private static readonly string[] _input = LoadInput(_classType).ToLines();

        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            ParseIngredients(_sampleInput).Item1.Should().Be(5);
            var result = ParseIngredients(_input);
            Console.WriteLine($"Non allergenIngredients: {result.Item1}");
        }

        private static void Problem2()
        {
            ParseIngredients(_sampleInput).Item2.Should().Be("mxmxvkd,sqjhc,fvjkl");
            var result = ParseIngredients(_input);
            Console.WriteLine($"Canonical List: {result.Item2}");
        }

        private static (int, string) ParseIngredients(string[] input)
        {
            var collection = new Dictionary<string, List<Ingredients>>();
            var ingredientCollection = new Dictionary<string, int>();
            foreach (var line in input)
            {
                var r = new Regex(@"\(contains (?<allergens>.*)\)");
                var ingredients = line.Split("(contains")[0]
                                      .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                      .ToList();
                ingredients.ForEach(ing =>
                {
                    if (!ingredientCollection.TryAdd(ing, 1))
                        ingredientCollection[ing]++;
                });
                var allergens = r.Match(line).Groups["allergens"].Value.Split(',', StringSplitOptions.TrimEntries).ToList();

                allergens.ForEach(a =>
                {
                    var addedKey = collection.TryAdd(a, ingredients.Select(ing => new Ingredients(ing)).ToList());
                    if (!addedKey)
                    {
                        collection[a] = collection[a].Where(p => ingredients.Contains(p.Ingredient)).ToList();
                    }
                });
            }

            var processed = new List<string>();
            while (collection.Values.Any(v => v.Count != 1))
            {
                var singleAllergens = collection.Where(d => d.Value.Count == 1 && !processed.Contains(d.Key));
                foreach (var kv in singleAllergens)
                {
                    var matchedValue = kv.Value.Select(s => s.Ingredient).Single();
                    foreach (var blah in collection.Keys.Where(k => k != kv.Key))
                    {
                        if (collection[blah].Any(p => p.Ingredient == matchedValue))
                        {
                            var ingredient = collection[blah].Single(s => s.Ingredient == matchedValue);
                            collection[blah].Remove(ingredient);
                        }
                    }
                    processed.Add(matchedValue);
                }
            }
            var nonAllergensCount = ingredientCollection.Where(s => !collection.Values.SelectMany(x => x.Select(z => z.Ingredient)).Any(i => i == s.Key)).Sum(k => k.Value);
            var canonicalList = string.Join(',', collection.OrderBy(kv => kv.Key)
                                                           .SelectMany(kv => kv.Value.Select(s => s.Ingredient)));
            return (nonAllergensCount, canonicalList);
        }

        private class Ingredients
        {
            public Ingredients(string ingredient)
            {
                Ingredient = ingredient;
                Count = 1;
            }
            public string Ingredient { get; }
            public int Count { get; set; }

            public static Ingredients operator ++(Ingredients a)
            {
                a.Count++;
                return a;
            }
        }
    }
}