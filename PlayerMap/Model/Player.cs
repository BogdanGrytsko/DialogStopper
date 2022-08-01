using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PlayerMap.Model.MasterPl;

namespace PlayerMap.Model
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string College { get; set; }
        public DateTime Debut { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Team { get; set; }
        //actually mongo
        public string Key { get; set; }
        public string League { get; set; }
        public string LeagueId { get; set; }
        public string Season { get; set; }
        public string SeasonId { get; set; }
        public Team TeamObj { get; set; }
        public int? Number { get; set; }

        public LeagueSeason GetLeagueSeason()
        {
            if (TeamObj == null)
                ParseTeam();
            
            return new LeagueSeason
            {
                League = new MonikerRef(LeagueId, League),
                Season = new BasicMonikerRef(SeasonId, Season),
                Player = new BasicMonikerRef(Id, GetName()),
                Team = new MonikerRef(TeamObj?.MongoId, TeamObj?.Name)
            };
        }

        public void ParseTeam()
        {
            TeamObj = JsonConvert.DeserializeObject<Team>(Team);
            if (TeamObj != null)
            {
                TeamObj.MongoId = string.IsNullOrEmpty(Team)
                    ? null
                    : Team.Split(",")[0].Split(new string[] { "oid", "}", ":", "\"", " ", "{", "$" },
                        StringSplitOptions.RemoveEmptyEntries)[1];
            }
        }
        
        //actually MySQl
        public int Iid { get; set; }
        public int NewPlayerIid { get; set; }
        public Player NewPlayer { get; set; }
        public Player ParentPlayer { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DSPlayerId { get; set; }

        public string GetName()
        {
            if (Id != null)
                return Name;
            return $"{FirstName} {LastName}";
        }
        
        public string GetKey()
        {
            var key = Name;
            // if (BirthDate >= new DateTime(1950, 1, 1))
            //     key += $"_{BirthDate}";
            // if (!string.IsNullOrWhiteSpace(BirthPlace) &&
            //     !BirthPlace.Equals("NA", StringComparison.InvariantCultureIgnoreCase))
            //     key += $"_{BirthPlace}";
            // if (!string.IsNullOrWhiteSpace(College) &&
            //     !College.Equals("NA", StringComparison.InvariantCultureIgnoreCase))
            //     key += $"_{College}";
            // if (Debut >= new DateTime(1950, 1, 1))
            //     key += $"_{Debut}";
            return key;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
                return $"{Name}";
            return $"{FirstName} {LastName}";
        }
            
        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        public Player Clone()
        {
            return (Player)MemberwiseClone();
        }
    }
}