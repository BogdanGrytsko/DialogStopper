using System;
using System.Collections.Generic;
using System.Linq;

namespace DialogStopper
{
    public class MeditationEntry
    {
        public DateTime TimeStamp { get; }
        public List<long> Points { get; }
        
        public int Count { get; }
        public double Avg { get; private set; }
        public double Min { get; private set; }
        public double Max { get; private set; }
        public double Std { get; private set; }

        public MeditationEntry(DateTime timeStamp, List<long> points)
        {
            TimeStamp = timeStamp;
            Points = points;
            Count = points.Count;
            Calculate();
        }

        private void Calculate()
        {
            var segments = GetSegments(Points);
            Avg = segments.Average();
            Min = segments.Min();
            Max = segments.Max();
            Std = StandardDeviation(segments);
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

        public static MeditationEntry FromTxtFile(string line)
        {
            var timeIdx = line.LastIndexOf(':');
            var arrIdx = line.IndexOf('.');
            var vals = line.Substring(arrIdx + 2).Split(',');
            return new MeditationEntry(DateTime.Parse(line.Substring(0, timeIdx)),
                vals.Select(long.Parse).ToList());
        }
    };
}