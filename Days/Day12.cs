using System;
using System.Collections.Generic;
using System.Drawing;
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
            var navSystem = new NavigationSystem(LoadInstructions(_sampleInput));
            navSystem.Execute().Should().Be(286);

            navSystem = new NavigationSystem(LoadInstructions(_input));
            Console.WriteLine($"Real Manhattan Distance: {navSystem.Execute()}");
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

        #endregion

        #region Part 2 classes

        private class NavigationSystem
        {
            private readonly List<Instruction> _navigationInstructions;
            private readonly WayPoint _wayPoint;
            private readonly Vessel _vessel;

            public NavigationSystem(IEnumerable<Instruction> instructions)
            {
                _navigationInstructions = instructions.ToList();
                _wayPoint = new WayPoint(new Point(10, 1));
                _vessel = new Vessel();
            }

            public int Execute()
            {
                foreach (var instruction in _navigationInstructions)
                {
                    switch (instruction.Command)
                    {
                        case Command.Forward:
                            _vessel.Move(instruction.Value, _wayPoint.Location);
                            break;
                        case Command.Left:
                        case Command.Right:
                            _wayPoint.Turn(instruction.Command, instruction.Value);
                            break;
                        case Command.Move:
                            _wayPoint.Move(instruction);
                            break;
                    }
                }

                return _vessel.ManhattanDistance;
            }
        }

        private class WayPoint
        {
            public WayPoint(Point initialLocation)
            {
                Location = initialLocation;
            }

            public Point Location { get; private set; }

            public void Move(Instruction instruction)
            {
                switch (instruction.Direction)
                {
                    case Direction.East:
                        Location = new Point(Location.X + instruction.Value, Location.Y);
                        break;
                    case Direction.South:
                        Location = new Point(Location.X, Location.Y - instruction.Value);
                        break;
                    case Direction.West:
                        Location = new Point(Location.X - instruction.Value, Location.Y);
                        break;
                    case Direction.North:
                        Location = new Point(Location.X, Location.Y + instruction.Value);
                        break;
                    default:
                        throw new ArgumentException($"Direction {instruction.Direction} is not a valid Move direction");
                }
            }

            public void Turn(Command direction, int degrees)
            {
                switch (direction)
                {
                    case Command.Right when degrees.Equals(90):
                    case Command.Left when degrees.Equals(270):
                        Location = Location.RotateOrthagonal(1);
                        break;
                    case Command.Right when degrees.Equals(270):
                    case Command.Left when degrees.Equals(90):
                        Location = Location.RotateOrthagonal(3);
                        break;
                    case Command.Right when degrees.Equals(180):
                    case Command.Left when degrees.Equals(180):
                        Location = Location.RotateOrthagonal(2);
                        break;
                    default:
                        throw new ArgumentException($"Command {direction} and {degrees} degrees are not valid for orthagonal rotation");
                }
            }
        }

        private class Vessel
        {
            private Point _location = new Point();

            public int ManhattanDistance => _location.GetManhattanDistance();

            public void Move(int times, Point wayPointLocation)
            {
                _location = new Point(_location.X + wayPointLocation.X * times,
                                      _location.Y + wayPointLocation.Y * times);
            }
        }

        #endregion
    }
}