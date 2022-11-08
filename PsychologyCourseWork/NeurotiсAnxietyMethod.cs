using System.Collections.Generic;

namespace PsychologyCourseWork
{
    public class NeuroticAnxietyMethod
    {
        private static readonly Dictionary<string, double> pointsMap = new()
        {
            { "Так", 1 },
            { "Ні", 0 },
            { "Не знаю", 0.5 },
            { "", 0.5 },
        };
        
        private static readonly Dictionary<string, double> negativePointsMap = new()
        {
            { "Так", 0 },
            { "Ні", 1 },
            { "Не знаю", 0.5 },
            { "", 0.5 },
        };
        
        public const int StartIndex = SocialPsychologicalRogersDiamondMethod.EndIndex + 1, AnxietyStartIndex = StartIndex + 42, EndIndex = StartIndex + 89;

        public NeuroticAnxietyMethod()
        {
        }
        
        public NeuroticAnxietyMethod(string[] values)
        {
            for (int i = StartIndex; i < AnxietyStartIndex; i++)
            {
                var val = values[i];
                var point = pointsMap[val];
                Neurotic += point;
            }
            var anxietyPlusIdx = AnxietyStartIndex + 37;
            for (int i = AnxietyStartIndex; i < EndIndex; i++)
            {
                var val = values[i];
                var point = i < anxietyPlusIdx ? pointsMap[val] : negativePointsMap[val];
                Anxiety += point;
            }
        }
        
        public double Neurotic { get; init; }
        public double Anxiety { get; init; }
    }
}