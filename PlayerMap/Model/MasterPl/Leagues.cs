using System;
using System.Collections.Generic;

namespace PlayerMap.Model.MasterPl
{
    public static class Leagues
    {
        public const int HighSchool = 1;
        public const int College = 2;
        public const int GrownUp = 3;
        
        public static Dictionary<string, string> HighSchoolLeagues { get; } = new Dictionary<string, string>
        {
            {"5526d40e4bc1ea55eb70f4ef", "Adidas"},
            {"571019cc50238a164768bf93", "Adidas Girls"},
            {"552d1a944bc1ea55eb70fb62", "Under Armour"},
            {"5cc74b677d52cb39ca8f614b", "Under Armour Girls"},
            {"54457dce300969b132fcfb40", "High School"},
            {"565d4542d269224c4b149c63", "High School Girls"},
            {"5f5a2894f68e52f827e180d8", "High School Summer Session"},
            {"5f88c49df68e52f827500aa2", "High School Girls Summer Session"},
            {"5526d40e4bc1ea55eb70f4ee", "Nike EYBL"},
            {"571beef550238a1647698b10", "Nike Girls EYBL"},
        };
        
        public static Dictionary<string, string> InternalLeagues { get; } = new Dictionary<string, string>
        {
            {"54457dce300969b132fcfb3c", "Synergy Basketball"},
            {"54457dce300969b132fcfb3e", "Training"},
            {"54457dce300969b132fcfb39", "FIBA - National Teams"},
            {"54457dce300969b132fcfb41", "FIBA - National Teams - Women"},
        };
        
        public static Dictionary<string, string> CollegeLeagues { get; } = new Dictionary<string, string>
        {
            {"54457dce300969b132fcfb37", "College Men"},
            {"54457dce300969b132fcfb38", "College Women"},
            {"584d07c1d95ef3f74c79ab90", "Junior College - Men"},
            {"584d07c1d95ef3f74c79ab91", "Junior College - Women"},
        };
        
        public static int GetLeagueValue(string leagueId)
        {
            if (HighSchoolLeagues.ContainsKey(leagueId))
                return HighSchool;
            if (CollegeLeagues.ContainsKey(leagueId))
                return College;
            return GrownUp;
        }

        public static string GetComment(int maxLeague, int newMaxLeague)
        {
            return $"{GetLeagueType(newMaxLeague)} after {GetLeagueType(maxLeague)}";
        }

        private static string GetLeagueType(int league)
        {
            return league switch
            {
                1 => "High School",
                2 => "College",
                3 => "Grown up",
                _ => throw new Exception("Unknown league type")
            };
        }
    }
}