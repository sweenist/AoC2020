using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day8
    {
        private static Type _classType = typeof(Day8);
        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            var instructionSet = new InstructionSet(LoadSample(_classType).ToLines());
            instructionSet.GetValueBeforeLoop().Should().Be(5);

            var result = new InstructionSet(LoadInput(_classType).ToLines()).GetValueBeforeLoop();
            Console.WriteLine($"Accumulated Value is {result}");
        }

        private static void Problem2()
        {
            Tests();
            var result = new InstructionSet(LoadInput(_classType).ToLines()).GetTerminatingAccumulator();
            Console.WriteLine($"Accumulated Value is {result}");
        }

        private static void Tests()
        {
            var instructionSet = new InstructionSet(LoadSample(_classType).ToLines());
            var nopInstruction = instructionSet.Instructions.First(inst => inst.Operation == Operation.Nop);
            nopInstruction.Swaperation();
            nopInstruction.Operation.Should().Be(Operation.Jmp);
            nopInstruction.Revert();
            nopInstruction.Operation.Should().Be(Operation.Nop);

            var jmpInstruction = instructionSet.Instructions.First(inst => inst.Operation == Operation.Jmp);
            jmpInstruction.Swaperation();
            jmpInstruction.Operation.Should().Be(Operation.Nop);
            jmpInstruction.Revert();
            jmpInstruction.Operation.Should().Be(Operation.Jmp);

            var accInstruction = instructionSet.Instructions.First(inst => inst.Operation == Operation.Acc);
            accInstruction.Swaperation();
            accInstruction.Operation.Should().Be(Operation.Acc);
            accInstruction.Revert();
            accInstruction.Operation.Should().Be(Operation.Acc);

            instructionSet.GetTerminatingAccumulator().Should().Be(8);
        }

        private enum Operation
        {
            Nop,
            Acc,
            Jmp,
        }

        private class InstructionSet
        {
            private int _accumulatedValue;
            private int _instructionIndex;

            public InstructionSet(string[] instructions)
            {
                var pattern = new Regex(@"(?<Operation>\w{3})\s(?<Value>[+|-]\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (var instruction in instructions)
                {
                    var match = pattern.Match(instruction);
                    if (!match.Success)
                        throw new ArgumentException($"Instruction invalid: {instruction}");

                    Instructions.Add(new Instruction(match.Groups["Operation"].Value, match.Groups["Value"].Value));
                }
            }

            public List<Instruction> Instructions { get; } = new List<Instruction>();
            public bool TerminatedNormally => _instructionIndex == Instructions.Count;

            public int GetValueBeforeLoop(List<int> instructionOrder = null)
            {
                while (true)
                {
                    if (TerminatedNormally)
                        break;

                    var instruction = Instructions[_instructionIndex];
                    if (instruction.CallCount.Equals(1))
                        break;
                    switch (instruction.Operation)
                    {
                        case Operation.Nop:
                            instructionOrder?.Add(_instructionIndex);
                            _instructionIndex++;
                            break;
                        case Operation.Jmp:
                            instructionOrder?.Add(_instructionIndex);
                            _instructionIndex = instruction.Perform(_instructionIndex);
                            break;
                        case Operation.Acc:
                            _accumulatedValue = instruction.Perform(_accumulatedValue);
                            _instructionIndex++;
                            break;
                        default:
                            throw new InvalidOperationException($"Instruction operation invalid: {instruction.Operation}");
                    }
                }
                return _accumulatedValue;
            }

            public int GetTerminatingAccumulator()
            {
                bool IsSingleInstructionInfiniteLoop(Instruction inst) => inst.Value.Equals(0);
                bool IsSingleJumpAhead(Instruction inst) => inst.Value.Equals(1);

                var instructionIndicies = GetOriginalInstructionIndicies();
                var index = 0;
                while (!TerminatedNormally)
                {
                    Reset();
                    var specificInstruction = Instructions.ElementAt(instructionIndicies[index]);
                    if (IsSingleInstructionInfiniteLoop(specificInstruction) || IsSingleJumpAhead(specificInstruction))
                    {
                        index++;
                        continue;
                    }
                    specificInstruction.Swaperation();
                    GetValueBeforeLoop();
                    index++;
                }
                Console.WriteLine($"Instruction on line {instructionIndicies[index - 1] + 1} changed.");
                return _accumulatedValue;
            }

            private List<int> GetOriginalInstructionIndicies()
            {
                var originalInstructionIndicies = new List<int>();
                GetValueBeforeLoop(originalInstructionIndicies);
                Reset();
                return originalInstructionIndicies;
            }

            private void Reset()
            {
                _accumulatedValue = 0;
                _instructionIndex = 0;
                Instructions.All(inst => inst.Revert());
            }
        }

        private class Instruction
        {
            private readonly Operation _originalOperation;
            public Instruction(string operation, string value)
            {
                Operation = Enum.Parse<Operation>(operation, ignoreCase: true);
                Value = Convert.ToInt32(value);
                _originalOperation = Operation;
            }
            public int CallCount { get; private set; }
            public Operation Operation { get; private set; }

            public int Value { get; }

            public int Perform(int input)
            {
                CallCount++;
                return input + Value;
            }

            public void Swaperation()
            {
                switch (Operation)
                {
                    case Operation.Nop:
                        Operation = Operation.Jmp;
                        break;
                    case Operation.Jmp:
                        Operation = Operation.Nop;
                        break;
                    default:
                        break;
                }
            }

            public bool Revert()
            {
                Operation = _originalOperation;
                CallCount = 0;
                return true;
            }
        }
    }
}