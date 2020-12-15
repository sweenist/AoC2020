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
        private static readonly string[] _sample2 = @"mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1".ToLines();
        private static readonly string[] _input = LoadInput(_classType).ToLines();
        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            InitializeMemory(_sampleInput, false).Should().Be(165);
            var result = InitializeMemory(_input, includeXMask: false);
            Console.WriteLine($"Total Memory = {result}");
        }

        private static void Problem2()
        {
            InitializeMemory(_sample2, includeXMask: true).Should().Be(208L);
            var result = InitializeMemory(_input, includeXMask: true);
            Console.WriteLine($"Total Float Memory = {result}");
        }

        private static long InitializeMemory(string[] input, bool includeXMask)
        {
            var memoryInitializer = new MemoryInitializer(input.First(), includeXMask);
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
            private readonly bool _includeXMask;
            public MemoryInitializer(string mask, bool includeXMask)
            {
                _includeXMask = includeXMask;
                ParseMask(mask.Split('=', StringSplitOptions.TrimEntries).Last());
            }

            public Dictionary<int, char> SchemeMask { get; private set; }

            public Dictionary<long, string> Addresses { get; } = new Dictionary<long, string>();

            private void ParseMask(string mask)
            {
                SchemeMask = mask.Select((c, i) => new KeyValuePair<int, char>(i, c))
                                 .Where(kvp => _includeXMask || kvp.Value != 'X')
                                 .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            private void MaskValue(Instruction instruction)
            {
                var binaryValue = instruction.Value.ToBinary(36).ToArray();
                foreach (var key in SchemeMask.Keys)
                {
                    binaryValue[key] = SchemeMask[key];
                }
                Addresses.AddOrOverwrite(instruction.Address, new string(binaryValue));
            }

            private void MaskFloatingValue(Instruction instruction)
            {
                const int bitDepth = 36;

                var targetValue = instruction.Value.ToBinary(bitDepth);
                var registryAddress = instruction.Address.ToBinary(bitDepth)
                                                 .Zip(SchemeMask.Select(s => s.Value),
                                                 (f, s) => s == '1' ? s : f)
                                                 .ToArray();

                var targetAddress = ((char[])registryAddress.Clone())
                                                            .Zip(SchemeMask.Select(kvp => kvp.Value),
                                                                                  (f, s) => s == 'X' ? '0' : f).ToArray();
                var indicies = SchemeMask.Where(m => m.Value == 'X').Select(m => m.Key).ToList();
                var interimBitDepth = indicies.Count;

                for (var i = 0; i < Math.Pow(2, interimBitDepth); ++i)
                {
                    var flipflop = i.ToInt64().ToBinary(interimBitDepth);
                    var _ = indicies.Zip(flipflop, (f, s) => targetAddress[f] = s).ToArray();

                    Addresses.AddOrOverwrite(new string(targetAddress).FromBinary(), targetValue);
                }
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
                        case BitType.Mem when !_includeXMask:
                            MaskValue(instruction);
                            break;
                        case BitType.Mem when _includeXMask:
                            MaskFloatingValue(instruction);
                            break;
                    }
                }
                return Addresses.Select(kvp => kvp.Value.FromBinary()).Sum();
            }
        }

        private struct Instruction
        {
            public BitType BitType { get; set; }
            public long Address { get; set; }
            public string BinaryValue { get; set; }
            public long Value { get; set; }
        }
    }
}