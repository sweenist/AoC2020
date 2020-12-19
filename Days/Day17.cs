using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public static class Day17
    {
        private static Type _classType = typeof(Day17);
        private static readonly IEnumerable<IEnumerable<bool>> _sampleInput = LoadSample(_classType).ToLines()
                                                                                                    .Select(s => s.CharToBool(c => c == '#'));

        private static readonly IEnumerable<IEnumerable<bool>> _input = LoadInput(_classType).ToLines()
                                                                                             .Select(s => s.CharToBool(c => c == '#'));

        public static void Run()
        {
            Problem1();
            Problem2();
        }

        private static void Problem1()
        {
            GenerateCubes<TriPoint>(_sampleInput);
            GenerateCubes<TriPoint>(_input);
        }

        private static void Problem2()
        {
            GenerateCubes<QuadPoint>(_sampleInput);
            GenerateCubes<QuadPoint>(_input);
        }

        private static void GenerateCubes<T>(IEnumerable<IEnumerable<bool>> cubeStates) where T : INPoint
        {
            var manager = new CubeManager<T>(cubeStates);

            for (var n = 0; n < 6; ++n)
            {
                manager.ExpandCubeSpace();
                manager.EnergyTransfer();
                Console.WriteLine($"Active cubes at step {n}: {manager.Cubes.Values.Count(c => c)}");
            }
            Console.WriteLine($"Total active cubes: {manager.Cubes.Values.Count(c => c)}");
            Console.WriteLine();
        }

        private class CubeManager<T> where T : INPoint
        {
            public CubeManager(IEnumerable<IEnumerable<bool>> cubeStates)
            {
                object[] GetParameters(int x, int y)
                {
                    var parameters = new object[2];
                    parameters[0] = x;
                    parameters[1] = y;
                    return parameters;
                }
                var constructor = typeof(T).GetConstructors().Single(c => c.GetParameters().Count() == 2);

                Cubes = cubeStates.Select((b1, i) => b1.Select((b2, j) => new KeyValuePair<INPoint, bool>((INPoint)constructor.Invoke(GetParameters(j, i)), b2)))
                                  .SelectMany(_ => _)
                                  .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            public Dictionary<INPoint, bool> Cubes { get; set; }

            public void ExpandCubeSpace()
            {
                var activePositions = Cubes.Where(p => p.Value)
                                           .Select(k => k.Key)
                                           .ToList();

                foreach (var pos in activePositions)
                    foreach (var v3 in pos.GetAdjacent())
                    {
                        Cubes.TryAdd(v3, false);
                    }
            }

            public void EnergyTransfer()
            {
                var cubesToActivate = new List<INPoint>();
                var cubesToTurnOff = new List<INPoint>();

                var cubePositions = Cubes.Select(k => k.Key)
                                         .ToList();

                foreach (var (pos, isActive) in Cubes)
                {
                    var count = 0;
                    var neighbours = pos.GetAdjacent();


                    foreach (var neighbour in neighbours)
                    {
                        if (Cubes.ContainsKey(neighbour) && Cubes[neighbour])
                            count++;
                        if (count > 3)
                        {
                            if (isActive)
                                cubesToTurnOff.Add(pos);
                            break;
                        }
                    }
                    if (isActive && count < 2)
                        cubesToTurnOff.Add(pos);
                    else if (!isActive && count == 3)
                        cubesToActivate.Add(pos);
                }

                cubesToActivate.ForEach(p => Cubes[p] = true);
                cubesToTurnOff.ForEach(p => Cubes[p] = false);
            }
        }

        private interface INPoint : IEquatable<INPoint>
        {
            List<int> Position { get; }
            IEnumerable<INPoint> GetAdjacent();
        }

        private class TriPoint : INPoint
        {
            public TriPoint(int x, int y)
            {
                X = x;
                Y = y;
            }

            public TriPoint(int x, int y, int z) : this(x, y)
            {
                Z = z;
            }

            public int X { get; }
            public int Y { get; }
            public int Z { get; }

            public List<int> Position => new[] { X, Y, Z }.ToList();

            public IEnumerable<INPoint> GetAdjacent()
            {

                foreach (var z in Enumerable.Range(-1, 3))
                    foreach (var y in Enumerable.Range(-1, 3))
                        foreach (var x in Enumerable.Range(-1, 3))
                            if (!(x == 0 && y == 0 && z == 0))
                                yield return new TriPoint(X + x, Y + y, Z + z);
            }

            public bool Equals(INPoint other)
            {
                if (!(other is TriPoint point))
                    return false;

                return X.Equals(point.X)
                    && Y.Equals(point.Y)
                    && Z.Equals(point.Z);
            }

            public override int GetHashCode()
            {
                const int seed = 7;
                const int modifier = 31;

                unchecked
                {
                    return Position.Aggregate(seed, (current, item) =>
                        (current * modifier) + item.GetHashCode());
                }
            }
        }

        private class QuadPoint : INPoint
        {
            public QuadPoint(int x, int y)
            {
                X = x;
                Y = y;
            }

            public QuadPoint(int w, int x, int y, int z) : this(x, y)
            {
                W = w;
                Z = z;
            }

            public int W { get; }
            public int X { get; }
            public int Y { get; }
            public int Z { get; }

            public List<int> Position => new[] { W, X, Y, Z }.ToList();

            public IEnumerable<INPoint> GetAdjacent()
            {
                foreach (var z in Enumerable.Range(-1, 3))
                    foreach (var y in Enumerable.Range(-1, 3))
                        foreach (var x in Enumerable.Range(-1, 3))
                            foreach (var w in Enumerable.Range(-1, 3))
                                if (!(w == 0 && x == 0 && y == 0 && z == 0))
                                    yield return new QuadPoint(W + w, X + x, Y + y, Z + z);

            }

            public bool Equals(INPoint other)
            {
                if (!(other is QuadPoint point))
                    return false;

                return W.Equals(point.W)
                    && X.Equals(point.X)
                    && Y.Equals(point.Y)
                    && Z.Equals(point.Z);
            }

            public override int GetHashCode()
            {
                const int seed = 7;
                const int modifier = 31;

                unchecked
                {
                    return Position.Aggregate(seed, (current, item) =>
                        (current * modifier) + item.GetHashCode());
                }
            }
        }
    }
}