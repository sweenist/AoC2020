using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    public static class Day2
    {
        private static Regex _regex = new Regex(@"(?<min>\d+)-(?<max>\d+)\s(?<character>\w):\s(?<password>.*)$",
                                                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string[] _inputs = File.ReadAllLines(".\\Days\\Inputs\\Day2.txt");
        private static List<PasswordCriteria> _criterion = _inputs.Select(line => new PasswordCriteria(_regex.Match(line).Groups)).ToList();

        private class PasswordCriteria
        {
            public PasswordCriteria(GroupCollection collection)
            {
                Min = Convert.ToInt32(collection["min"].Value);
                Max = Convert.ToInt32(collection["max"].Value);
                EssentialCharacter = Convert.ToChar(collection["character"].Value);
                Password = collection["password"].Value;
            }

            public int Min { get; }
            public int Max { get; }
            public char EssentialCharacter { get; }
            public string Password { get; }

            private int EssentialCharacterCount => Password.Count(c => c.Equals(EssentialCharacter));

            public override string ToString() => $"Min: {Min}; Max: {Max}; Character: {EssentialCharacter}; Password {Password}; IsValid {MeetsPart1Criteria}";

            public bool MeetsPart1Criteria => Min <= EssentialCharacterCount && EssentialCharacterCount <= Max;

            public bool MeetsPart2Criteria => Password[Min - 1].Equals(EssentialCharacter) ^ Password[Max - 1].Equals(EssentialCharacter);
        }
        public static int Problem1()
        {
            return _criterion.Where(p => p.MeetsPart1Criteria).Count();
        }

        public static int Problem2()
        {
            return _criterion.Where(p => p.MeetsPart2Criteria).Count();
        }

        public static void TestProblem2()
        {
            var testCases = new Dictionary<string, bool>{
                {"1-3 a: abcde", true},
                {"1-3 b: cdefg", false},
                {"2-9 c: ccccccccc", false},
            };

            foreach (var key in testCases.Keys)
            {
                var target = new PasswordCriteria(_regex.Match(key).Groups);
                if (target.MeetsPart2Criteria != testCases[key])
                    Console.WriteLine($"ERROR: {key} criteria should be {testCases[key]}");
            }
        }
    }
}