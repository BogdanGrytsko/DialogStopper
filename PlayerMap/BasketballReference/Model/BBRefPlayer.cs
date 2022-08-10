using System;

namespace PlayerMap.BasketballReference.Model
{
    public class BBRefPlayer
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Pos { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthCountry { get; set; }
        public string YearsOfExperience { get; set; }
        public string College { get; set; }
        public string CollegeUrl { get; set; }
        public string Class { get; set; }



        public string TeamName { get; set; }
        public string MongoTeamId { get; set; }
        public string SeasonName { get; set; }
        public string MongoSeasonId { get; set; }
        public string RSCITop100 { get; set; }
        public string Summary { get; set; }

        public override string ToString()
        {
            return $"{Number} {Name}";
        }
    }
}