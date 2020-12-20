using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day18
    {
        private static Type _classType = typeof(Day18);
        private static readonly string[] _sampleInput = LoadSample(_classType).Replace(" ", string.Empty).ToLines();

        private static readonly string[] _input = LoadInput(_classType).Replace(" ", string.Empty).ToLines();

        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            Parsinate(_sampleInput).Should().Be(26 + 437 + 12240 + 13632);
            var result = Parsinate(_input);
            Console.WriteLine($"Sum of all maths is {result}");
        }

        private static void Problem2()
        {
            Parsinate(_sampleInput, true).Should().Be(46 + 1445 + 669060 + 23340);
            var result = Parsinate(_input, true);
            Console.WriteLine($"Sum of unprecendented maths is {result}");
        }

        private static long Parsinate(string[] expressions, bool useBackwardsPrecedence = false)
        {
            var returnValue = 0L;
            foreach (var expression in expressions)
            {
                returnValue += ParseExpression(expression, useBackwardsPrecedence);
            }
            return returnValue;
        }

        private static long ParseExpression(string expression, bool useBackwardsPrecedence)
        {
            var availableKeys = Enumerable.Range(65, 26).Concat(Enumerable.Range(97, 26)).Select(i => (char)i).ToArray();
            var maskedValues = new Dictionary<char, long>();
            var keyIndex = -1;

            var parenthetical = new Regex(@"(?<equation>\(([\d\w][\*\+])+[\d\w]\))");
            var parentheticalMatch = parenthetical.Match(expression);
            while (parentheticalMatch.Success)
            {
                var rawValue = parentheticalMatch.Groups["equation"].Value;
                var replacementCopy = rawValue;
                for (var i = 0; i < rawValue.Count(c => c == '*' || c == '+'); ++i)
                {
                    try
                    {
                        var additionIndex = replacementCopy.IndexOf('+') - 1;
                        var substring = useBackwardsPrecedence && additionIndex > 0
                            ? replacementCopy.Substring(additionIndex, 3)
                            : replacementCopy.Substring(1, 3);
                        var maths = new Equation(substring[0], substring[1], substring[2], maskedValues);

                        keyIndex++;
                        maskedValues[availableKeys[keyIndex]] = maths.Result;
                        replacementCopy = replacementCopy.ReplaceFirst(substring, availableKeys[keyIndex].ToString());
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine($"Uh-oh, {rawValue} or {replacementCopy} is malformed");
                        throw e;
                    }
                }

                expression = expression.Replace(rawValue, availableKeys[keyIndex].ToString());
                parentheticalMatch = parenthetical.Match(expression);
            }

            var basic = new Regex(@"^[\d\w][\*\+][\d\w]");
            var basicMatch = basic.Match(expression);

            while (basicMatch.Success)
            {
                for (var i = 0; i < expression.Count(c => c == '*' || c == '+'); ++i)
                {
                    var additionIndex = expression.IndexOf('+') - 1;
                    var substring = useBackwardsPrecedence && additionIndex >= 0
                        ? expression.Substring(additionIndex, 3)
                        : expression.Substring(0, 3);
                    var maths = new Equation(substring[0], substring[1], substring[2], maskedValues);

                    keyIndex++;
                    maskedValues[availableKeys[keyIndex]] = maths.Result;
                    expression = expression.ReplaceFirst(substring, availableKeys[keyIndex].ToString());
                }
                basicMatch = basic.Match(expression);
            }
            return maskedValues[availableKeys[keyIndex]];
        }

        private enum Operation
        {
            None,
            Add,
            Multiply
        }

        private struct Equation
        {
            public Equation(char left, char operation, char right, Dictionary<char, long> valueMap)
            {
                Left = Char.IsDigit(left) ? Convert.ToInt64(left.ToString()) : valueMap[left];
                Right = Char.IsDigit(right) ? Convert.ToInt64(right.ToString()) : valueMap[right];
                Operation = operation switch
                {
                    '+' => Operation.Add,
                    '*' => Operation.Multiply,
                    _ => Operation.None
                };
            }

            public long Left { get; }
            public Operation Operation { get; }
            public long Right { get; }

            public long Result
            {
                get
                {
                    switch (Operation)
                    {
                        case Operation.Add:
                            return Left + Right;
                        case Operation.Multiply:
                            return Left * Right;
                        default:
                            throw new InvalidOperationException("Operation is invalid");
                    }
                }
            }
        }
    }
}