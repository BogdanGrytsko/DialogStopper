namespace PlayerMap.Model.Scrape
{
    public class NbaPlayer
    {
        public string TeamName { get; set; }
        public string Username { get; set; }
        public string PlayerUrl { get; set; }
        public string Weight { get; set; }
        public string Height { get; set; }
        public string Born { get; set; }
        public string College { get; set; }
        public string HighSchool { get; set; }
        public string NBADebut { get; set; }
        public string Experience { get; set; }
        public override string ToString()
        {
            return $"{TeamName},{Username},{PlayerUrl}";
        }
    }
}