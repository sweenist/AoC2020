using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;
using static AdventOfCode.Utils.BinaryExtensions;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    public static class Day14
    {
        private static Type _classType = typeof(Day14);
        private static readonly string[] _sampleInput = LoadSample(_classType).ToLines();
        private static readonly string[] _input = LoadInput(_classType).ToLines();
        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            InitializeMemory(_sampleInput).Should().Be(165);
            var result = InitializeMemory(_input);
            Console.WriteLine($"Total Memory = {result}");
        }

        private static void Problem2()
        {
        }

        private static long InitializeMemory(string[] input)
        {
            var memoryInitializer = new MemoryInitializer(input.First());
            var instructions = input.Skip(1)
                                    .Select(ParseInstruction);
            return memoryInitializer.Execute(instructions);
        }

        private static Instruction ParseInstruction(string input)
        {
            var maskPattern = new Regex(@"mask\s=\s(?<maskValue>[01X]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var memPattern = new Regex(@"mem\[(?<address>\d+)\]\s=\s(?<value>\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var maskMatch = maskPattern.Match(input);
            var memMatch = memPattern.Match(input);

            if (maskMatch.Success)
            {
                return new Instruction { BitType = BitType.Mask, BinaryValue = maskMatch.Groups["maskValue"].Value };
            }
            if (memMatch.Success)
            {
                return new Instruction
                {
                    BitType = BitType.Mem,
                    Value = Convert.ToInt64(memMatch.Groups["value"].Value),
                    Address = Convert.ToInt32(memMatch.Groups["address"].Value),
                };
            }
            throw new ArgumentException($"input does not match: {input}");
        }

        private enum BitType
        {
            Mask,
            Mem
        }

        private class MemoryInitializer
        {
            public MemoryInitializer(string mask)
            {
                ParseMask(mask.Split('=', StringSplitOptions.TrimEntries).Last());
            }

            public Dictionary<int, char> SchemeMask { get; private set; }

            public Dictionary<int, string> Addresses { get; } = new Dictionary<int, string>();

            public void ParseMask(string mask)
            {
                SchemeMask = mask.Select((c, i) => new KeyValuePair<int, char>(i, c))
                                 .Where(kvp => kvp.Value != 'X')
                                 .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            public void MaskValue(Instruction instruction)
            {
                var binaryValue = instruction.Value.ToBinary(36).ToArray();
                foreach (var key in SchemeMask.Keys)
                {
                    binaryValue[key] = SchemeMask[key];
                }
                Addresses.AddOrOverwrite(instruction.Address, new string(binaryValue));
            }

            public long Execute(IEnumerable<Instruction> instructions)
            {
                foreach (var instruction in instructions)
                {
                    switch (instruction.BitType)
                    {
                        case BitType.Mask:
                            ParseMask(instruction.BinaryValue);
                            break;
                        case BitType.Mem:
                            MaskValue(instruction);
                            break;
                    }
                }
                return Addresses.Select(kvp => kvp.Value.FromBinary()).Sum();
            }
        }

        private struct Instruction
        {
            public BitType BitType { get; set; }
            public int Address { get; set; }
            public string BinaryValue { get; set; }
            public long Value { get; set; }
        }
    }
}