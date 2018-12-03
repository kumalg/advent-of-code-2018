﻿/*
--- Day 3: No Matter How You Slice It ---

The Elves managed to locate the chimney-squeeze prototype fabric for Santa's suit (thanks to someone who helpfully wrote its box IDs on the wall of the warehouse in the middle of the night). Unfortunately, anomalies are still affecting them - nobody can even agree on how to cut the fabric.

The whole piece of fabric they're working on is a very large square - at least 1000 inches on each side.

Each Elf has made a claim about which area of fabric would be ideal for Santa's suit. All claims have an ID and consist of a single rectangle with edges parallel to the edges of the fabric. Each claim's rectangle is defined as follows:

    The number of inches between the left edge of the fabric and the left edge of the rectangle.
    The number of inches between the top edge of the fabric and the top edge of the rectangle.
    The width of the rectangle in inches.
    The height of the rectangle in inches.

A claim like #123 @ 3,2: 5x4 means that claim ID 123 specifies a rectangle 3 inches from the left edge, 2 inches from the top edge, 5 inches wide, and 4 inches tall. Visually, it claims the square inches of fabric represented by # (and ignores the square inches of fabric represented by .) in the diagram below:

...........
...........
...#####...
...#####...
...#####...
...#####...
...........
...........
...........

The problem is that many of the claims overlap, causing two or more claims to cover part of the same areas. For example, consider the following claims:

#1 @ 1,3: 4x4
#2 @ 3,1: 4x4
#3 @ 5,5: 2x2

Visually, these claim the following areas:

........
...2222.
...2222.
.11XX22.
.11XX22.
.111133.
.111133.
........

The four square inches marked with X are claimed by both 1 and 2. (Claim 3, while adjacent to the others, does not overlap either of them.)

If the Elves all proceed with their own plans, none of them will have enough fabric. How many square inches of fabric are within two or more claims?

Your puzzle answer was 109143.
--- Part Two ---

Amidst the chaos, you notice that exactly one claim doesn't overlap by even a single square inch of fabric with any other claim. If you can somehow draw attention to it, maybe the Elves will be able to make Santa's suit after all!

For example, in the claims above, only claim 3 is intact after all claims are made.

What is the ID of the only claim that doesn't overlap?

Your puzzle answer was 506.

Both parts of this puzzle are complete! They provide two gold stars: **
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2018.Days {
    public class Day03 {
        public static async Task Solve() {
            Console.WriteLine("Day 3");

            var ids = (await File.ReadAllLinesAsync("../../../Inputs/day_03.txt"))
                .Select(x => Regex.Split(x, @"[@,:x]"))
                .ToList();

            Console.WriteLine(PartOne(ids));
            Console.WriteLine(PartTwo(ids));
            Console.ReadKey();
        }

        private static int PartOne(IList<string[]> ids) {
            var fabric = new int[1000, 1000];
            foreach (var id in ids) {
                var left = int.Parse(id[1]);
                var width = int.Parse(id[3]);
                var top = int.Parse(id[2]);
                var height = int.Parse(id[4]);
                for (var x = left; x < left + width; x++) {
                    for (var y = top; y < top + height; y++) {
                        fabric[y, x] = fabric[y, x] + 1;
                    }
                }
            }

            return fabric.Cast<int>().Count(x => x > 1);
        }

        private static int PartTwo(IList<string[]> ids) {
            var fabric = new string[1000, 1000];
            var overlaped = new bool[ids.Count];
            foreach (var id in ids) {
                var left = int.Parse(id[1]);
                var width = int.Parse(id[3]);
                var top = int.Parse(id[2]);
                var height = int.Parse(id[4]);
                for (var x = left; x < left + width; x++) {
                    for (var y = top; y < top + height; y++) {
                        if (string.IsNullOrEmpty(fabric[y, x]))
                            fabric[y, x] = id[0];
                        else {
                            if (fabric[y, x] != "X")
                                overlaped[int.Parse(fabric[y, x].Substring(1)) - 1] = true;
                            overlaped[int.Parse(id[0].Substring(1)) - 1] = true;
                            fabric[y, x] = "X";
                        }
                    }
                }
            }

            return overlaped
                .Select((x, i) => (x, i))
                .FirstOrDefault(x => !x.Item1)
                .Item2 + 1;
        }
    }
}
