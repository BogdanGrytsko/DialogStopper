using System;
using System.Collections.Generic;
using System.IO;

namespace PsychologyCourseWork
{
    public class LifeEventsMethod
    {
        private static readonly Dictionary<int, int> pointsMap = new();

        public LifeEventsMethod()
        {
        }
        
        public LifeEventsMethod(string[] values)
        {
            for (int i = NeuroticAnxietyMethod.EndIndex; i < values.Length; i++)
            {
                var val = values[i];
                try
                {
                    var count = GetCount(val);
                    var point = pointsMap[i - NeuroticAnxietyMethod.EndIndex + 1];
                    LifeEventResult += count * point;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static int GetCount(string val)
        {
            if (string.IsNullOrEmpty(val)) return 0;
            switch (val)
            {
                case "ні":
                case "-":
                    return 0;
                case "так":
                    return 1;
                case "Багато":
                    return 10;
                default:
                    return int.Parse(val);
            }
        }
        
        public int LifeEventResult { get; init; }

        public static void Load()
        {
            var sep = ';';
            var path = @"C:\temp\LifeEventsPoints.txt";
            var lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var values = line.Split(sep);
                pointsMap.Add(i + 1, int.Parse(values[1]));
            }
        }
    }
}