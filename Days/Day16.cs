using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day16
    {
        private static Type _classType = typeof(Day16);
        private static readonly string[] _sampleInput = LoadSample(_classType).ToLines();
        private static readonly string[] _sample2 = @"class: 0-1 or 4-19
row: 0-5 or 8-19
seat: 0-13 or 16-19

your ticket:
11,12,13

nearby tickets:
3,9,18
15,1,5
5,14,9".ToLines();
        private static readonly string[] _input = LoadInput(_classType).ToLines();
        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            var sampleTicketInfo = ParseTickets(_sampleInput.ToList());
            sampleTicketInfo.ScanInvalidTickets().Should().Be(71);

            var realTicketInfo = ParseTickets(_input.ToList());
            Console.WriteLine($"Error number: {realTicketInfo.ScanInvalidTickets()}");
        }

        private static void Problem2()
        {
            // var sampleTicketInfo = ParseTickets(_sample2.ToList());
            // sampleTicketInfo.OrderTicketFields();

            var realTicketInfo = ParseTickets(_input.ToList());
            realTicketInfo.OrderTicketFields();
        }

        private static TicketInfo ParseTickets(List<string> input)
        {
            const string myTicketHeader = "your ticket:";
            const string nearbyTicketHeader = "nearby tickets:";
            var boundRegex = new Regex(@"(?<className>.*):\s(?<min1>\d+)-(?<max1>\d+)\sor\s(?<min2>\d+)-(?<max2>\d+)", RegexOptions.Compiled);

            var rules = input.Where(s => boundRegex.Match(s).Success)
                             .Select(s =>
                             {
                                 var groups = boundRegex.Match(s).Groups;
                                 return new TicketRules(groups.Value("className"),
                                                        groups.Int32Value("min1"),
                                                        groups.Int32Value("max1"),
                                                        groups.Int32Value("min2"),
                                                        groups.Int32Value("max2"));
                             });
            var myTicket = new Ticket(input[input.IndexOf(myTicketHeader) + 1]);
            var nearByTickets = input.Skip(input.IndexOf(nearbyTicketHeader) + 1)
                                     .Select(s => new Ticket(s));

            return new TicketInfo(rules, myTicket, nearByTickets);
        }

        private class TicketInfo
        {
            public TicketInfo(IEnumerable<TicketRules> rules, Ticket myTicket, IEnumerable<Ticket> nearyByTickets)
            {
                FieldBounds = rules.ToList();
                Tickets = nearyByTickets.ToList();
                MyTicket = myTicket;
            }

            public List<TicketRules> FieldBounds { get; }
            public List<Ticket> Tickets { get; }
            public List<Ticket> ValidTickets => Tickets.Where(
                                                        t => t.Values.All(
                                                            v => FieldBounds.Any(
                                                                fb => fb.IsValueValid(v))))
                                                        .ToList();
            public Ticket MyTicket { get; }

            public int ScanInvalidTickets()
            {
                return Tickets.SelectMany(t => t.Values)
                              .Where(i => !FieldBounds.Any(fb => fb.IsValueValid(i)))
                              .Sum();
            }

            public void OrderTicketFields()
            {
                var validClasses = new List<KeyValuePair<string, int>>();
                var headerRows = new Dictionary<string, int>();
                var valueCount = ValidTickets.Max(v => v.Values.Count);

                for (var i = 0; i < valueCount; ++i)
                {
                    var columnValues = ValidTickets.Select(t => t.Values[i]);
                    validClasses.AddRange(FieldBounds.Where(fb => fb.IsRuleValidForValues(columnValues))
                                                     .Select(fb => new KeyValuePair<string, int>(fb.ClassName, i)));
                }
                while (validClasses.Count > 0)
                {
                    validClasses.GroupBy(kvp => kvp.Key)
                                .Where(g => g.Count() == 1)
                                .SelectMany(g => g.Select(k => k))
                                .ToList().ForEach(kvp => headerRows.AddKvp(kvp));
                    validClasses.RemoveAll(kvp => headerRows.ContainsValue(kvp.Value));
                }

                var result = headerRows.Where(kv => kv.Key.StartsWith("departure"))
                                       .Select(kv => MyTicket.Values[kv.Value])
                                       .Aggregate(1L, (f, s) => f * s);
            }
        }

        internal record TicketRules(string ClassName, int Min1, int Max1, int Min2, int Max2)
        {
            public bool IsValueValid(int value) => value.Includes(Min1, Max1) || value.Includes(Min2, Max2);

            public bool IsRuleValidForValues(IEnumerable<int> values) => values.All(v => IsValueValid(v));
        }

        private class Ticket
        {
            public Ticket(string values)
            {
                Values = values.Split(',').Select(v => Convert.ToInt32(v)).ToList();
            }
            public List<int> Values { get; }
        }
    }
}