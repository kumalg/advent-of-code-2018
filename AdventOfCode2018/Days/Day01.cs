using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2018.Days {
    public class Day01 {
        public static async Task Solve() {
            Console.WriteLine("Day 1");

            var changesString = await File.ReadAllLinesAsync("../../../Inputs/day_01.txt");
            var changes = changesString.Select(int.Parse).ToList();

            Console.WriteLine(PartOne(changes));
            Console.WriteLine(PartTwo(changes));
            Console.ReadKey();
        }

        private static int PartOne(IEnumerable<int> changes) {
            return changes.Sum();
        }

        private static int PartTwo(ICollection<int> changes) {
            var frequencies = new HashSet<int> { 0 };
            var current = 0;
            var index = 0;
            while (true) {
                current += changes.ElementAt(index++ % changes.Count);
                if (!frequencies.Add(current))
                    return current;
            }
        }
    }
}
