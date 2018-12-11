﻿/*
--- Day 11: Chronal Charge ---

You watch the Elves and their sleigh fade into the distance as they head toward the North Pole.

Actually, you're the one fading. The falling sensation returns.

The low fuel warning light is illuminated on your wrist-mounted device. Tapping it once causes it to project a hologram of the situation: a 300x300 grid of fuel cells and their current power levels, some negative. You're not sure what negative power means in the context of time travel, but it can't be good.

Each fuel cell has a coordinate ranging from 1 to 300 in both the X (horizontal) and Y (vertical) direction. In X,Y notation, the top-left cell is 1,1, and the top-right cell is 300,1.

The interface lets you select any 3x3 square of fuel cells. To increase your chances of getting to your destination, you decide to choose the 3x3 square with the largest total power.

The power level in a given fuel cell can be found through the following process:

    Find the fuel cell's rack ID, which is its X coordinate plus 10.
    Begin with a power level of the rack ID times the Y coordinate.
    Increase the power level by the value of the grid serial number (your puzzle input).
    Set the power level to itself multiplied by the rack ID.
    Keep only the hundreds digit of the power level (so 12345 becomes 3; numbers with no hundreds digit become 0).
    Subtract 5 from the power level.

For example, to find the power level of the fuel cell at 3,5 in a grid with serial number 8:

    The rack ID is 3 + 10 = 13.
    The power level starts at 13 * 5 = 65.
    Adding the serial number produces 65 + 8 = 73.
    Multiplying by the rack ID produces 73 * 13 = 949.
    The hundreds digit of 949 is 9.
    Subtracting 5 produces 9 - 5 = 4.

So, the power level of this fuel cell is 4.

Here are some more example power levels:

    Fuel cell at  122,79, grid serial number 57: power level -5.
    Fuel cell at 217,196, grid serial number 39: power level  0.
    Fuel cell at 101,153, grid serial number 71: power level  4.

Your goal is to find the 3x3 square which has the largest total power. The square must be entirely within the 300x300 grid. Identify this square using the X,Y coordinate of its top-left fuel cell. For example:

For grid serial number 18, the largest total 3x3 square has a top-left corner of 33,45 (with a total power of 29); these fuel cells appear in the middle of this 5x5 region:

-2  -4   4   4   4
-4   4   4   4  -5
 4   3   3   4  -4
 1   1   2   4  -3
-1   0   2  -5  -2

For grid serial number 42, the largest 3x3 square's top-left is 21,61 (with a total power of 30); they are in the middle of this region:

-3   4   2   2   2
-4   4   3   3   4
-5   3   3   4  -4
 4   3   3   4  -3
 3   3   3  -5  -1

What is the X,Y coordinate of the top-left fuel cell of the 3x3 square with the largest total power?

Your puzzle answer was 235,35.
--- Part Two ---

You discover a dial on the side of the device; it seems to let you select a square of any size, not just 3x3. Sizes from 1x1 to 300x300 are supported.

Realizing this, you now must find the square of any size with the largest total power. Identify this square by including its size as a third parameter after the top-left coordinate: a 9x9 square with a top-left corner of 3,5 is identified as 3,5,9.

For example:

    For grid serial number 18, the largest total square (with a total power of 113) is 16x16 and has a top-left corner of 90,269, so its identifier is 90,269,16.
    For grid serial number 42, the largest total square (with a total power of 119) is 12x12 and has a top-left corner of 232,251, so its identifier is 232,251,12.

What is the X,Y,size identifier of the square with the largest total power?

Your puzzle answer was 142,265,7.

Both parts of this puzzle are complete! They provide two gold stars: **
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2018.Days {
    public class Day10 {
        public static async Task Solve() {
            Console.WriteLine("Day 10");

            var points = (await File.ReadAllLinesAsync("../../../Inputs/day_10.txt"))
                .Select(x => Regex.Match(x, @"position=<(?<pX>.*), (?<pY>.*)> velocity=<(?<vX>.*), (?<vY>.*)>").Groups.Skip(1).Select(y => int.Parse(y.Value)).ToList())
                .Select(x => new Point { Position = (x[0], x[1]), Velocity = (x[2], x[3]) });

            PartOne(points, 10800);
            Console.ReadKey();
        }

        private static void PartOne(IEnumerable<Point> points, int seconds) {
            var bestPoints = points.Select(x => x.NextPosition(seconds)).ToList().AsEnumerable();
            var minSurfaceArea = CountSurfaceArea(bestPoints);

            while (true) {
                seconds++;
                var newPoints = bestPoints.Select(x => x.NextPosition());
                var newSurfaceArea = CountSurfaceArea(newPoints);

                if (newSurfaceArea <= minSurfaceArea) {
                    minSurfaceArea = newSurfaceArea;
                    bestPoints = newPoints;
                } else {
                    Message(bestPoints);
                    Console.WriteLine(seconds - 1);
                    return;
                }
            }
        }

        private static bool IsFucked(IEnumerable<Point> points) {
            return Math.Abs(points.Max(x => x.Position.X) - points.Min(x => x.Position.X)) < 200
                || Math.Abs(points.Max(y => y.Position.Y) - points.Min(y => y.Position.Y)) < 200;
        }

        private static long CountSurfaceArea(IEnumerable<Point> points) {
            return Math.Abs((points.Max(x => x.Position.X) - points.Min(x => x.Position.X)) * (points.Max(y => y.Position.Y) - points.Min(y => y.Position.Y)));
        }

        private static bool Message(IEnumerable<Point> points) {
            var minX = points.Min(x => x.Position.X);
            var maxX = points.Max(x => x.Position.X);
            var minY = points.Min(y => y.Position.Y);
            var maxY = points.Max(y => y.Position.Y);

            var matrix = new bool[maxX - minX + 1, maxY - minY + 1];

            foreach (var point in points) {
                matrix[point.Position.X - minX, point.Position.Y - minY] = true;
            }

            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);

            for (int i = 0; i < colLength; i++) {
                for (int j = 0; j < rowLength; j++) {
                    Console.Write(matrix[j, i] ? "#" : ".");
                }
                Console.WriteLine();
            }

            return true;
        }
    }

    public class Point {
        public (long X, long Y) Position { get; set; }
        public (long X, long Y) Velocity { get; set; }

        public Point NextPosition() {
            return new Point {
                Position = (Position.X + Velocity.X, Position.Y + Velocity.Y),
                Velocity = (Velocity.X, Velocity.Y)
            };
        }
        public Point NextPosition(int x) {
            return new Point {
                Position = (Position.X + Velocity.X * x, Position.Y + Velocity.Y * x),
                Velocity = (Velocity.X, Velocity.Y)
            };
        }
    }
}
