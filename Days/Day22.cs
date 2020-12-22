using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day22
    {
        private static Dictionary<string, int> _recursionChecker;
        private static Dictionary<int, int> _rounds;
        private static int _game;
        private static Type _classType = typeof(Day22);
        private static readonly List<string> _sampleInput = LoadSample(_classType).ToLines(StringSplitOptions.TrimEntries).ToList();

        private static readonly List<string> _input = LoadInput(_classType).ToLines(StringSplitOptions.TrimEntries).ToList();

        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            PlayRound(_sampleInput).Should().Be(306);

            var result = PlayRound(_input);
            Console.WriteLine($"The winning hand aggregate: {result}");
        }

        private static void Problem2()
        {
            // PlayRound(_sampleInput, recurse: true).Should().Be(291);

            var result = ElapsedAction(() => PlayRound(_input, recurse: true));
            Console.WriteLine($"The winning hand of Recursive Combat: {result}");
        }

        private static long PlayRound(List<string> input, bool recurse = false)
        {
            var (player1Deck, player2Deck) = Setup(input);
            _recursionChecker = new Dictionary<string, int>();
            _rounds = new Dictionary<int, int>();
            _game = 1;

            var adjective = recurse ? "Recursive" : string.Empty;
            Console.WriteLine($"Starting new game of {adjective} Combat");

            Play(player1Deck, player2Deck, recurse, _game);
            Console.WriteLine();

            return player1Deck.Concat(player2Deck)
                              .Reverse()
                              .Select((card, i) => (long)card * (i + 1))
                              .Sum();
        }

        private static int Play(List<int> p1Deck, List<int> p2Deck, bool recurse, int game)
        {
            var key = string.Join(',', p1Deck) + '|' + string.Join(',', p2Deck);
            if (_recursionChecker.ContainsKey(key))
            {
                Console.WriteLine("Found a recursive instance. skipping...");
                return _recursionChecker[key];
            }
            var recurses = new[] { key }.ToList();

            while (p1Deck.Any() && p2Deck.Any())
            {
                var playedSubGame = false;
                var p1Card = p1Deck.First();
                var p2Card = p2Deck.First();

                if (!_rounds.ContainsKey(game))
                    _rounds.Add(game, 0);
                _rounds[game]++;

                p1Deck.RemoveAt(0);
                p2Deck.RemoveAt(0);

                var trick = new[] { p1Card, p2Card }.OrderByDescending(c => c).ToList();
                var winner = p1Card > p2Card ? 1 : 2;

                if (recurse && p1Card <= p1Deck.Count && p2Card <= p2Deck.Count)
                {
                    playedSubGame = true;
                    Console.WriteLine($"\tGame {game} round {_rounds[game]} Playing a subgame...");
                    winner = Play(p1Deck.Take(p1Card).ToList(), p2Deck.Take(p2Card).ToList(), recurse, ++_game);
                    trick = winner.Equals(1)
                        ? new[] { p1Card, p2Card }.ToList()
                        : new[] { p2Card, p1Card }.ToList();
                }

                _recursionChecker[key] = winner;
                if (!playedSubGame)
                    Console.WriteLine($"Game {game} Round {_rounds[game]} Cards Played {p1Card}, {p2Card}: Player {winner} wins!");

                if (winner.Equals(1))
                    p1Deck.AddRange(trick);
                else
                    p2Deck.AddRange(trick);

                var newKey = string.Join(',', p1Deck) + '|' + string.Join(',', p2Deck);
                recurses.Add(newKey);
                if (recurses.Count(r => r.Equals(newKey)) > 1)
                    return 1;
            }
            return p1Deck.Any() ? 1 : 2;
        }

        private static (List<int>, List<int>) Setup(List<string> input)
        {
            List<int> CreateHand(int skipDepth) => input.Skip(skipDepth)
                                                                    .TakeWhile(s => !string.IsNullOrWhiteSpace(s))
                                                                    .Select(s => Convert.ToInt32(s))
                                                                    .ToList();

            var player1Deck = CreateHand(1);
            var player2Deck = CreateHand(input.IndexOf("Player 2:") + 1);

            return (player1Deck, player2Deck);
        }
    }
}