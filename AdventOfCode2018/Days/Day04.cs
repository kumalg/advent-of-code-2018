/*
--- Day 4: Repose Record ---

You've sneaked into another supply closet - this time, it's across from the prototype suit manufacturing lab. You need to sneak inside and fix the issues with the suit, but there's a guard stationed outside the lab, so this is as close as you can safely get.

As you search the closet for anything that might help, you discover that you're not the first person to want to sneak in. Covering the walls, someone has spent an hour starting every midnight for the past few months secretly observing this guard post! They've been writing down the ID of the one guard on duty that night - the Elves seem to have decided that one guard was enough for the overnight shift - as well as when they fall asleep or wake up while at their post (your puzzle input).

For example, consider the following records, which have already been organized into chronological order:

[1518-11-01 00:00] Guard #10 begins shift
[1518-11-01 00:05] falls asleep
[1518-11-01 00:25] wakes up
[1518-11-01 00:30] falls asleep
[1518-11-01 00:55] wakes up
[1518-11-01 23:58] Guard #99 begins shift
[1518-11-02 00:40] falls asleep
[1518-11-02 00:50] wakes up
[1518-11-03 00:05] Guard #10 begins shift
[1518-11-03 00:24] falls asleep
[1518-11-03 00:29] wakes up
[1518-11-04 00:02] Guard #99 begins shift
[1518-11-04 00:36] falls asleep
[1518-11-04 00:46] wakes up
[1518-11-05 00:03] Guard #99 begins shift
[1518-11-05 00:45] falls asleep
[1518-11-05 00:55] wakes up

Timestamps are written using year-month-day hour:minute format. The guard falling asleep or waking up is always the one whose shift most recently started. Because all asleep/awake times are during the midnight hour (00:00 - 00:59), only the minute portion (00 - 59) is relevant for those events.

Visually, these records show that the guards are asleep at these times:

Date   ID   Minute
            000000000011111111112222222222333333333344444444445555555555
            012345678901234567890123456789012345678901234567890123456789
11-01  #10  .....####################.....#########################.....
11-02  #99  ........................................##########..........
11-03  #10  ........................#####...............................
11-04  #99  ....................................##########..............
11-05  #99  .............................................##########.....

The columns are Date, which shows the month-day portion of the relevant day; ID, which shows the guard on duty that day; and Minute, which shows the minutes during which the guard was asleep within the midnight hour. (The Minute column's header shows the minute's ten's digit in the first row and the one's digit in the second row.) Awake is shown as ., and asleep is shown as #.

Note that guards count as asleep on the minute they fall asleep, and they count as awake on the minute they wake up. For example, because Guard #10 wakes up at 00:25 on 1518-11-01, minute 25 is marked as awake.

If you can figure out the guard most likely to be asleep at a specific time, you might be able to trick that guard into working tonight so you can have the best chance of sneaking in. You have two strategies for choosing the best guard/minute combination.

Strategy 1: Find the guard that has the most minutes asleep. What minute does that guard spend asleep the most?

In the example above, Guard #10 spent the most minutes asleep, a total of 50 minutes (20+25+5), while Guard #99 only slept for a total of 30 minutes (10+10+10). Guard #10 was asleep most during minute 24 (on two days, whereas any other minute the guard was asleep was only seen on one day).

While this example listed the entries in chronological order, your entries are in the order you found them. You'll need to organize them before they can be analyzed.

What is the ID of the guard you chose multiplied by the minute you chose? (In the above example, the answer would be 10 * 24 = 240.)

Your puzzle answer was 14346.
--- Part Two ---

Strategy 2: Of all guards, which guard is most frequently asleep on the same minute?

In the example above, Guard #99 spent minute 45 asleep more than any other guard or minute - three times in total. (In all other cases, any guard spent any minute asleep at most twice.)

What is the ID of the guard you chose multiplied by the minute you chose? (In the above example, the answer would be 99 * 45 = 4455.)

Your puzzle answer was 5705.

Both parts of this puzzle are complete! They provide two gold stars: **
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2018.Days {
    public class Day04 {
        public static async Task Solve() {
            Console.WriteLine("Day 2");

            var logs = await File.ReadAllLinesAsync("../../../Inputs/day_04.txt");

            Console.WriteLine(PartOne(logs));
            Console.WriteLine(PartTwo(logs));
            Console.ReadKey();
        }

        private static double PartOne(IReadOnlyCollection<string> ids) {
            var logs = ids
                .Select(x => Regex.Split(x, @"[\[\]]").Where(y => !string.IsNullOrEmpty(y)))
                .Select((x, index) => new {
                    Date = DateTime.Parse(x.ElementAt(0)),
                    Message = x.ElementAt(1)
                })
                .OrderBy(x => x.Date)
                .Select((x, index) => new {
                    Index = index,
                    x.Date,
                    x.Message,
                })
                .ToList();

            var guards = logs
                .Where(x => x.Message.Trim().StartsWith("Guard"))
                .Select(x => new {
                    GuardId = int.Parse(x.Message.Trim().Replace("Guard #", "").Replace(" begins shift", "")),
                    History = logs.Skip(x.Index + 1).TakeWhile(y => !y.Message.Trim().StartsWith("Guard")).ToList()
                })
                .Select(x => new {
                    x.GuardId,
                    x.History,
                    Minutes = CountMinutes(x.History.Select(y => (y.Date, y.Message)).ToList())
                })
                .GroupBy(x => x.GuardId)
                .Select(x => new {
                    GuardId = x.Key,
                    Minutes = x.Select(y => y.Minutes).Sum(),
                    History = x.Select(y => y.History)
                });

            var most = guards.OrderByDescending(x => x.Minutes).FirstOrDefault();

            var minute = most.History.Select(x => MinuteRange(x.Select(y => (y.Date, y.Message)).ToList()))
                .SelectMany(x => x)
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .First()
                .Key;

            return most.GuardId * minute;
        }

        private static double PartTwo(IReadOnlyCollection<string> ids) {
            var logs = ids
                .Select(x => Regex.Split(x, @"[\[\]]").Where(y => !string.IsNullOrEmpty(y)))
                .Select((x, index) => new {
                    Date = DateTime.Parse(x.ElementAt(0)),
                    Message = x.ElementAt(1)
                })
                .OrderBy(x => x.Date)
                .Select((x, index) => new {
                    Index = index,
                    x.Date,
                    x.Message,
                })
                .ToList();

            var guard = logs
                .Where(x => x.Message.Trim().StartsWith("Guard"))
                .Select(x => new {
                    GuardId = int.Parse(x.Message.Trim().Replace("Guard #", "").Replace(" begins shift", "")),
                    History = logs.Skip(x.Index + 1).TakeWhile(y => !y.Message.Trim().StartsWith("Guard")).ToList()
                })
                .Select(x => new {
                    x.GuardId,
                    x.History,
                    Minutes = CountMinutes(x.History.Select(y => (y.Date, y.Message)).ToList())
                })
                .GroupBy(x => x.GuardId)
                .Select(x => new {
                    GuardId = x.Key,
                    BestMinute = BestMinute(x.Select(y => y.History)
                        .Select(y => y.Select(z => (z.Date, z.Message)).ToList()).ToList())
                })
                .OrderByDescending(x => x.BestMinute.Item2)
                .First();

            return guard.GuardId * guard.BestMinute.Item1;
        }

        private static int CountMinutes(IList<(DateTime, string)> history) {
            return history.Select((x, i) => {
                if (i % 2 == 0) return -x.Item1.Minute;
                return x.Item1.Minute;
            }).Sum();
        }

        private static (int, int) BestMinute(IList<List<(DateTime, string)>> history) {
            return history.Select(x => MinuteRange(x.Select(y => (y.Item1, y.Item2)).ToList()))
                .SelectMany(x => x)
                .GroupBy(x => x)
                .Select(x => (x.Key, x.Count()))
                .OrderByDescending(x => x.Item2)
                .FirstOrDefault();
        }

        private static IList<int> MinuteRange(IList<(DateTime, string)> history) {
            return history
                .Select((x, i) => (Value: x, Index: i))
                .GroupBy(x => x.Index / 2)
                .Select(x => Enumerable.Range(x.First().Value.Item1.Minute,
                    x.Last().Value.Item1.Minute - x.First().Value.Item1.Minute))
                .SelectMany(x => x)
                .ToList();
        }
    }
}
