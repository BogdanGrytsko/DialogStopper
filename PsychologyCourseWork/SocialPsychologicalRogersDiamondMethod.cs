using System;
using System.Collections.Generic;
using System.Linq;

namespace PsychologyCourseWork
{
    public class SocialPsychologicalRogersDiamondMethod
    {
        private static readonly Dictionary<string, int> pointsMap = new()
        {
            { "", 3 },
            { "це до мене абсолютно не відноситься", 0 },
            { "сумніваюся, що це можна віднести до мене", 2 },
            { "не наважуюся віднести це до себе", 3 },
            { "це схоже на мене, але немає впевненості", 4 },
            { "це схоже на мене, але не впевнений/на", 4 },
            { "це на мене схоже", 5 },
            { "це точно про мене", 6 },
        };
        private const int startIndex = 4, endIndex = startIndex + 101;

        private static readonly int[] adaptation = new[]
        {
            4, 5, 9, 12, 15, 19, 22, 23, 26, 27, 29, 33, 35, 37, 41, 44, 47, 51, 53, 55, 61, 63, 67, 72, 74, 75, 78, 80,
            88, 91, 94, 96, 97, 98
        };
        private static readonly int[] disAdaptation = new[]
        {
            2, 6, 7, 13, 16, 18, 25, 28, 32, 36, 38, 40, 42, 43, 49, 50, 54, 56, 59, 60, 62, 64, 69, 71, 73, 76, 77, 83,
            84, 86, 90, 95, 99, 100
        };
        private static readonly int[] dishonesty = new[]
        {
            34, 45, 48, 81, 89, 8, 82, 92, 101
        };
        private static readonly int[] selfAcceptance = new[]
        {
            33, 35, 55, 67, 72, 74, 75, 80, 88, 94, 96
        };
        private static readonly int[] selfDisAcceptance = new[]
        {
            7, 59, 62, 65, 90, 95, 99 
        };
        private static readonly int[] othersAcceptance = new[]
        {
            9, 14, 22, 26, 53, 97
        };
        private static readonly int[] othersDisAcceptance = new[]
        {
            2, 10, 21, 28, 40, 60, 76
        };
        private static readonly int[] emotionalComfort = new[]
        {
            23, 29, 30, 41, 44, 47, 78
        };
        private static readonly int[] emotionalDisComfort = new[]
        {
            6, 42, 43, 49, 50, 83, 85
        };
        private static readonly int[] innerControl = new[]
        {
            4, 5, 11, 12, 13, 19, 27, 37, 51, 63, 68, 79, 91, 98
        };
        private static readonly int[] outerControl = new[]
        {
            25, 36, 52, 57, 70, 71, 73, 77
        };
        private static readonly int[] domination = new[]
        {
            58, 61, 66
        };
        private static readonly int[] submission = new[]
        {
            16, 32, 38, 69, 84, 87
        };
        private static readonly int[] escapism = new[]
        {
            17, 18, 54, 64, 86
        };
        
        private readonly Dictionary<int, int> result = new();

        public SocialPsychologicalRogersDiamondMethod()
        {
        }

        public SocialPsychologicalRogersDiamondMethod(string[] values, string[] headers)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                var val = values[i];
                var point = pointsMap[val];
                result.Add(i - startIndex + 1, point);
            }

            Adaptation = adaptation.Sum(x => result[x]);
            DisAdaptation = disAdaptation.Sum(x => result[x]);
            Dishonesty = dishonesty.Sum(x => result[x]);
            SelfAcceptance = selfAcceptance.Sum(x => result[x]);
            SelfDisAcceptance = selfDisAcceptance.Sum(x => result[x]);
            OthersAcceptance = othersAcceptance.Sum(x => result[x]);
            OthersDisAcceptance = othersDisAcceptance.Sum(x => result[x]);
            EmotionalComfort = emotionalComfort.Sum(x => result[x]);
            EmotionalDisComfort = emotionalDisComfort.Sum(x => result[x]);
            InnerControl = innerControl.Sum(x => result[x]);
            OuterControl = outerControl.Sum(x => result[x]);
            Domination = domination.Sum(x => result[x]);
            Submission = submission.Sum(x => result[x]);
            Escapism = escapism.Sum(x => result[x]);

            AdaptationCalc = Math.Round((double)Adaptation / (Adaptation + DisAdaptation) * 100);
            SelfAcceptanceCalc = Math.Round((double)SelfAcceptance / (SelfAcceptance + SelfDisAcceptance) * 100);
            OthersAcceptanceCalc = Math.Round((double)OthersAcceptance * 1.2 / (OthersAcceptance * 1.2 + OthersDisAcceptance) * 100);
            EmotionalComfortCalc = Math.Round((double)EmotionalComfort / (EmotionalComfort + EmotionalDisComfort) * 100);
            InternalityCalc = Math.Round((double)InnerControl / (InnerControl + OuterControl * 1.4 ) * 100);
            DominationCalc = Math.Round((double)Domination * 2 / (Domination * 2 + Submission) * 100);
        }
        
        public int Adaptation { get; init; }
        public int DisAdaptation { get; init; }
        public int Dishonesty { get; init; }
        public int SelfAcceptance { get; init; }
        public int SelfDisAcceptance { get; init; }
        public int OthersAcceptance { get; init; }
        public int OthersDisAcceptance { get; init; }
        public int EmotionalComfort { get; init; }
        public int EmotionalDisComfort { get; init; }
        public int InnerControl { get; init; }
        public int OuterControl { get; init; }
        public int Domination { get; init; }
        public int Submission { get; init; }
        public int Escapism { get; init; }
        
        public double AdaptationCalc { get; init; }
        public double SelfAcceptanceCalc { get; init; }
        public double OthersAcceptanceCalc { get; init; }
        public double EmotionalComfortCalc { get; init; }
        public double InternalityCalc { get; init; }
        public double DominationCalc { get; init; }
    }
}