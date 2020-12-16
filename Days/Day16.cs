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
        private static readonly string[] _input = LoadInput(_classType).ToLines();
        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            var sampleTicketInfo = ParseTickets(_sampleInput.ToList());
            sampleTicketInfo.FieldBounds.Count.Should().Be(3);
            sampleTicketInfo.Tickets.Count.Should().Be(4);
            sampleTicketInfo.MyTicket.Values.Should().ContainInOrder(7,1,14);
        }

        private static void Problem2()
        {
        }

        private static TicketInfo ParseTickets(List<string> input)
        {
            const string myTicketHeader = "your ticket:";
            const string nearbyTicketHeader = "nearby tickets:";
            var boundRegex = new Regex(@"(?<className>.*):\s(?<min1>\d+)-(?<max1>\d+).*(?<min2>\d+)-(?<max2>\d+)", RegexOptions.Compiled);

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
            public Ticket MyTicket { get; }
        }

        internal record TicketRules(string ClassName, int Min1, int Max1, int Min2, int Min3)
        {

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