namespace PlayerMap.BasketballReference.Model
{
    public class TeamSeasonDto
    {
        public string BBRefTeamId { get; set; }
        public int BBRefSeason { get; set; }
        public string SynergySeasonId { get; set; }
        public string SynergyTeamId { get; set; }

        public override string ToString()
        {
            return $"{BBRefTeamId}, {BBRefSeason}";
        }
    }
}