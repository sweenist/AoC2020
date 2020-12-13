using System;
using System.Drawing;

public static class GraphExtensions
{
    public static Point RotateOrthagonal(this Point point, int steps)
    {
        switch (steps)
        {
            case 0:
                return point;
            case 1: // Clockwise
                return new Point(point.Y, -point.X);
            case 2:
                return new Point(-point.X, -point.Y);
            case 3: // Counterclockwise
                return new Point(-point.Y, point.X);
            default:
                throw new ArgumentException($"steps needs to be 0-3. Value was {steps}");
        }
    }

    public static int GetManhattanDistance(this Point point)
    {
        return Math.Abs(point.X) + Math.Abs(point.Y);
    }
}