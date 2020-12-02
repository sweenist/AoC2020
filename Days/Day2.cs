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

            public override string ToString() => $"Min: {Min}; Max: {Max}; Character: {EssentialCharacter}; Password {Password}; IsValid {IsValid}";

            public bool IsValid => Min <= EssentialCharacterCount && EssentialCharacterCount <= Max;
        }
        public static int Problem1()
        {
            var passwords = new List<PasswordCriteria>();
            using (var reader = new StreamReader(".\\Days\\Inputs\\Day2.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    passwords.Add(new PasswordCriteria(_regex.Match(line).Groups));
                }
            }

            return passwords.Where(p => p.IsValid).Count();
        }
    }
}