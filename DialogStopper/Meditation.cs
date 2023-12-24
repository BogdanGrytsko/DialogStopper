using Newtonsoft.Json;

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
        public double Sum { get; private set; }
        public double Total { get; private set; }
        public double Percent { get; private set; }
        public List<(long, PointType)> Points { get; private set; }

        public Meditation(DateTime timeStamp, List<(long, PointType)> points)
        {
            TimeStamp = timeStamp;
            Points = points;
            Calculate();
        }

        //used in reflection
        public Meditation()
        {
        }

        private void Calculate()
        {
            var segments = GetSegments(Points);
            Avg = Math.Round(segments.Average());
            Min = segments.Min();
            Max = segments.Max();
            Std = Math.Round(StandardDeviation(segments));
            Sum = segments.Sum();
            Total = Points.Last().Item1;
            Percent = Math.Round(Sum / Total * 100);
        }
        
        private static double StandardDeviation(ICollection<long> sequence)
        {
            if (!sequence.Any()) return 0;
            
            var average = sequence.Average();
            var sum = sequence.Sum(d => Math.Pow(d - average, 2));
            var result = Math.Sqrt(sum / (sequence.Count - 1));
            return result;
        }

        private static List<long> GetSegments(List<(long, PointType)> points)
        {
            var segments = new List<long>();
            var prev = 0L; 
            foreach (var (time, pointType) in points)
            {
                //todo: focused eyes training log
                if (pointType == PointType.SingleThought)
                {
                    var segment = time - prev;
                    segments.Add(segment);
                }
                else if (pointType == PointType.ContinuousSegment)
                {
                }
                prev = time;
            }
            return segments;
        }

        public static Meditation FromTxtFile(string line)
        {
            return JsonConvert.DeserializeObject<Meditation>(line);
        }
    };
}