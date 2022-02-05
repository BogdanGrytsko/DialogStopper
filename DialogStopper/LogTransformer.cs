using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DialogStopper
{
    public class LogTransformer
    {
        public static void ToGoogleSheets(string logFile)
        {
            //min, max, avg, sum, std
            var lines = File.ReadAllLines(logFile);
            var entries = GetEntries(lines).ToList();
            using var sw = new StreamWriter("SheetLog.txt");
            sw.WriteLine($"Time\t{GetHeaders(entries)}");
            for (int i = 0; i < entries.Count; i++)
            {
                AddEntry(sw, entries[i], i + 1);

            }
            sw.Close();
        }

        private static void AddEntry(StreamWriter sw, MeditationEntry entry, int i)
        {
            foreach (var entryValue in entry.Values)
            {
                sw.WriteLine($"{entryValue}{GetSeparator(i)}{i}");
            }
        }

        private static string GetSeparator(int i)
        {
            var s = string.Empty;
            for (int j = 0; j < i; j++)
            {
                s += '\t';
            }
            return s;
        }

        private static string GetHeaders(List<MeditationEntry> entries)
        {
            return string.Join('\t', entries.Select(x => x.Time.ToString("MM/dd/yyyy")));
        }

        private static IEnumerable<MeditationEntry> GetEntries(string[] lines)
        {
            foreach (var line in lines)
            {
                var timeIdx = line.LastIndexOf(':');
                var arrIdx = line.IndexOf('.');
                var vals = line.Substring(arrIdx + 2).Split(',');
                yield return new MeditationEntry(DateTime.Parse(line.Substring(0, timeIdx)),
                    vals.Select(x => int.Parse(x)).ToArray());
            }
        }
    }
}