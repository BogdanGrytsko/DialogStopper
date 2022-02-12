using System;
using System.Collections.Generic;
using System.Linq;

namespace DialogStopper
{
    public class Meditation
    {
        public DateTime TimeStamp { get; private set; }
        public int Count => Points?.Count ?? 0;
        public double Avg { get; private set; }
        public double Min { get; private set; }
        public double Max { get; private set; }
        public double Std { get; private set; }
        public List<long> Points { get; private set; }

        public Meditation(DateTime timeStamp, List<long> points)
        {
            TimeStamp = timeStamp;
            Points = points;
            Calculate();
        }

        //used in reflection
        public Meditation()
        {
        }

        public void Calculate()
        {
            var segments = GetSegments(Points);
            Avg = Math.Round(segments.Average());
            Min = segments.Min();
            Max = segments.Max();
            Std = Math.Round(StandardDeviation(segments));
        }
        
        private static double StandardDeviation(ICollection<long> sequence)
        {
            if (!sequence.Any()) return 0;
            
            var average = sequence.Average();
            var sum = sequence.Sum(d => Math.Pow(d - average, 2));
            var result = Math.Sqrt(sum / (sequence.Count - 1));
            return result;
        }

        private static List<long> GetSegments(IEnumerable<long> points)
        {
            var segments = new List<long>();
            var prev = 0l; 
            foreach (var p in points)
            {
                var segment = p - prev;
                segments.Add(segment);
                prev = p;
            }
            return segments;
        }

        public static Meditation FromTxtFile(string line)
        {
            var timeIdx = line.LastIndexOf(':');
            var arrIdx = line.IndexOf('.');
            var vals = line.Substring(arrIdx + 2).Split(',');
            return new Meditation(DateTime.Parse(line.Substring(0, timeIdx)),
                vals.Select(long.Parse).ToList());
        }
    };
}