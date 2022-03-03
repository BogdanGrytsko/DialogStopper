using CsvHelper.Configuration;

namespace PlayerMap.Model
{
    public class MySqlPlayerMap : ClassMap<Player>
    {
        public MySqlPlayerMap()
        {
            Map(m => m.PlayerIid).Name("iPlayerID");
            Map(m => m.NewPlayerIid).Name("iNewPlayerID");
            Map(m => m.FirstName).Name("strPlayerFName");
            Map(m => m.LastName).Name("strPlayerLName");
            Map(m => m.Height).Name("iPlayerHeight");
            Map(m => m.Weight).Name("iPlayerWeight");
            Map(m => m.DSPlayerId).Name("iIDSPlayerID");
        }
    }
}