using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day12
    {
        private static Type _classType = typeof(Day12);
        private static readonly IEnumerable<string> _sampleInput = LoadSample(_classType).ToLines();
        private static readonly IEnumerable<string> _input = LoadInput(_classType).ToLines();
        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            RunSampleProblem1();

            var instructions = LoadInstructions(_input);
            var nav = new Navigation(instructions);
            nav.ExecuteInstructions();
            Console.WriteLine($"Manhattan Distance: {nav.ManhattanDistance}");
        }

        private static void RunSampleProblem1()
        {
            var instructions = LoadInstructions(_sampleInput);
            var nav = new Navigation(instructions);
            nav.ExecuteInstructions();
            nav.ManhattanDistance.Should().Be(25);
        }

        private static void Problem2()
        {

        }

        private static IEnumerable<Instruction> LoadInstructions(IEnumerable<string> instructions)
        {
            var pattern = new Regex(@"(?<command>\w)(?<value>\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            foreach (var instruction in instructions)
            {
                var match = pattern.Match(instruction);
                if (!match.Success)
                    throw new ArgumentException($"instruction {instruction} does not match pattern.");

                var command = match.Groups["command"].Value[0];
                var value = Convert.ToInt32(match.Groups["value"].Value);

                yield return new Instruction(command, value);
            }
        }

        private enum Direction
        {
            East,
            South,
            West,
            North
        }

        private enum Command
        {
            Undefined,
            Forward,
            Right,
            Left,
            Move
        }

        private class Instruction
        {
            public Instruction(char command, int value)
            {
                Command = command switch
                {
                    'F' => Command.Forward,
                    'R' => Command.Right,
                    'L' => Command.Left,
                    _ => Command.Move
                };
                Direction = command switch
                {
                    'E' => Direction.East,
                    'S' => Direction.South,
                    'W' => Direction.West,
                    'N' => Direction.North,
                    _ => Direction.East,
                };
                Value = value;
            }

            public Command Command { get; }
            public Direction Direction { get; }
            public int Value { get; }
        }

        #region Part 1 classes

        private class Navigation
        {
            private int _latitude = 0;
            private int _longitude = 0;
            private Direction _heading;
            private List<Instruction> _instructions;
            private Dictionary<Direction, Action<int>> _movementVectors = new Dictionary<Direction, Action<int>>();

            public Navigation(IEnumerable<Instruction> instructions)
            {
                _heading = Direction.East;
                _instructions = instructions.ToList();

                _movementVectors[Direction.East] = (v) => _latitude += v;
                _movementVectors[Direction.South] = (v) => _longitude -= v;
                _movementVectors[Direction.West] = (v) => _latitude -= v;
                _movementVectors[Direction.North] = (v) => _longitude += v;
            }

            public Direction Heading
            {
                get => _heading;
                private set
                {
                    if (value < 0)
                    {
                        _heading = value + 4; //Left movement
                        return;
                    }
                    _heading = (Direction)((int)value % 4);
                }
            }

            public int ManhattanDistance => Math.Abs(_latitude) + Math.Abs(_longitude);

            public void ExecuteInstructions()
            {
                foreach (var instruction in _instructions)
                {
                    switch (instruction.Command)
                    {
                        case Command.Forward:
                            _movementVectors[Heading](instruction.Value);
                            break;
                        case Command.Left:
                            Heading -= ((instruction.Value / 90) + 4) % 4;
                            break;
                        case Command.Right:
                            Heading += (instruction.Value / 90) % 4;
                            break;
                        case Command.Move:
                            _movementVectors[instruction.Direction](instruction.Value);
                            break;
                    }
                }
            }
        }

        private class NavigationSystem
        {

        }

        #endregion
    }
}