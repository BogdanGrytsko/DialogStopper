using System.Collections.Generic;

namespace PsychologyCourseWork
{
    public class NeuroticAnxietyMethod
    {
        private static readonly Dictionary<string, int> pointsMap = new()
        {
            { "Так", 1 },
            { "Ні", 0 },
            { "Не знаю", 0 },
            { "", -1 },
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
            for (int i = AnxietyStartIndex; i < EndIndex; i++)
            {
                var val = values[i];
                var point = pointsMap[val];
                Anxiety += point;
            }
        }
        
        public int Neurotic { get; init; }
        public int Anxiety { get; init; }
    }
}