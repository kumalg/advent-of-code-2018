/*
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
using System.Threading.Tasks;

namespace AdventOfCode2018.Days {
    public class Day13
    {
        public static Dictionary<char, Direction> CharDirections = new Dictionary<char, Direction> {
            { '>', Direction.Right },
            { '<', Direction.Left },
            { '^', Direction.Up },
            { 'v', Direction.Down }
        };
        public static Dictionary<Direction, (int X, int Y)> DirectionValues = new Dictionary<Direction, (int X, int Y)> {
            { Direction.Right, (1, 0) },
            { Direction.Left, (-1, 0) },
            { Direction.Up, (0, -1) },
            { Direction.Down, (0, 1)}
        };

        public static async Task Solve() {
            Console.WriteLine("Day 13");

            var lines = (await File.ReadAllLinesAsync("../../../Inputs/day_13.txt"))
                .Select(x => x.ToCharArray());

            var carts = lines
                .Select((line, yInd) => (line.Select((c, xInd) => (Char: c, Position: (xInd, yInd))), yInd))
                .SelectMany(x => x.Item1)
                .Where(x => CharDirections.Keys.Contains(x.Char))
                .Select((x, id) => new Cart {
                    Id = id,
                    Position = x.Position,
                    Direction = CharDirections[x.Char]
                })
                .ToList();

            var matrix = lines.Select(x => x.Select(y =>
                y == '<' || y == '>'
                    ? '-'
                    : y == '^' || y == 'v'
                        ? '|'
                        : y).ToArray()).ToArray();

            //PrintMatrixWithCars(matrix, carts);
            //Console.WriteLine(PartOne(matrix, carts));
            Console.WriteLine(PartTwo(matrix, carts));
            Console.ReadKey();
        }

        private static void PrintMatrixWithCars(char[][] matrix, IList<Cart> carts) {
            Console.WriteLine();
            for (var y = 0; y < matrix.Length; y++)
            {
                for (var x = 0; x < matrix[y].Length; x++)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    var cartsThere = carts.Where(a => a.Position == (x, y));
                    if (cartsThere.Count() > 1)
                        Console.Write('X');
                    else if (cartsThere.Count() == 1) {
                        Console.Write(CharDirections.First(a => a.Value == cartsThere.First().Direction).Key);
                    }
                    else {
                        Console.ResetColor();
                        Console.Write(matrix[y][x]);
                    }
                }
                Console.WriteLine();
            }
        }

        private static (int, int) PartOne(char[][] matrix, IList<Cart> carts) {
            while (true) {
                foreach (var cart in carts) {
                    cart.NextPosition();
                    var nextTrackPos = matrix[cart.Position.Y][cart.Position.X];
                    switch (nextTrackPos) {
                        case '+':
                            cart.Rotate();
                            break;
                        case '/':
                            switch (cart.Direction) {
                                case Direction.Up:
                                    cart.Direction = Direction.Right;
                                    break;
                                case Direction.Down:
                                    cart.Direction = Direction.Left;
                                    break;
                                case Direction.Left:
                                    cart.Direction = Direction.Down;
                                    break;
                                case Direction.Right:
                                    cart.Direction = Direction.Up;
                                    break;
                            }
                            break;
                        case '\\':
                            switch (cart.Direction) {
                                case Direction.Up:
                                    cart.Direction = Direction.Left;
                                    break;
                                case Direction.Down:
                                    cart.Direction = Direction.Right;
                                    break;
                                case Direction.Left:
                                    cart.Direction = Direction.Up;
                                    break;
                                case Direction.Right:
                                    cart.Direction = Direction.Down;
                                    break;
                            }
                            break;
                        case ' ':
                            throw new Exception();
                    }

                    var where = carts.GroupBy(x => x.Position).Where(x => x.Count() > 1);
                    if (where.Any())
                        return where.First().Key;
                }
            }
        }


        private static (int, int) PartTwo(char[][] matrix, IList<Cart> carts)
        {
            var ooo = 0;
            while (true)
            {
                ooo++;
                foreach (var cart in carts) {
                    if (cart.Crashed)
                        continue;

                    cart.NextPosition();
                    var nextTrackPos = matrix[cart.Position.Y][cart.Position.X];
                    switch (nextTrackPos) {
                        case '+':
                            cart.Rotate();
                            break;
                        case '/':
                            switch (cart.Direction) {
                                case Direction.Up:
                                    cart.Direction = Direction.Right;
                                    break;
                                case Direction.Down:
                                    cart.Direction = Direction.Left;
                                    break;
                                case Direction.Left:
                                    cart.Direction = Direction.Down;
                                    break;
                                case Direction.Right:
                                    cart.Direction = Direction.Up;
                                    break;
                            }
                            break;
                        case '\\':
                            switch (cart.Direction) {
                                case Direction.Up:
                                    cart.Direction = Direction.Left;
                                    break;
                                case Direction.Down:
                                    cart.Direction = Direction.Right;
                                    break;
                                case Direction.Left:
                                    cart.Direction = Direction.Up;
                                    break;
                                case Direction.Right:
                                    cart.Direction = Direction.Down;
                                    break;
                            }
                            break;
                        case ' ':
                            throw new Exception();
                    }

                    var crashedWith = carts
                        .Where(x => !x.Crashed)
                        .Where(x => x.Id != cart.Id)
                        .Where(x => x.Position == cart.Position)
                        .ToList();

                    if (crashedWith.Any())
                    {
                        cart.Crashed = true;
                        foreach (var cart1 in crashedWith)
                        {
                            cart1.Crashed = true;
                        }
                    }

                    if (carts.Count(x => !x.Crashed) == 1)
                    {
                        return carts.First(x => !x.Crashed).Position;
                    }
                }
            }
        }

        public class Cart {
            public int Id { get; set; }
            public bool Crashed { get; set; }
            public Direction Direction { get; set; }
            public Turn LastRotateDirection { get; set; } = Turn.Right;
            public (int X, int Y) Position { get; set; }

            public void Rotate() {
                switch (LastRotateDirection) {
                    case Turn.Right:
                        Direction = (Direction)(((int)Direction + 3 ) % 4);
                        LastRotateDirection = Turn.Left;
                        break;
                    case Turn.Left:
                        LastRotateDirection = Turn.Straight;
                        break;
                    default:
                        Direction = (Direction)(((int)Direction + 1) % 4);
                        LastRotateDirection = Turn.Right;
                        break;
                }
            }

            public void NextPosition() {
                Position = (
                    Position.X + DirectionValues[Direction].X,
                    Position.Y + DirectionValues[Direction].Y);
            }
        }

        public enum Direction {
            Up, Right, Down, Left
        }

        public enum Turn {
            Left, Straight, Right
        }
    }
}
