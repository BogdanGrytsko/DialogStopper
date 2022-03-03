using System.Collections.Generic;
using System.Linq;
using DialogStopper.Storage;
using PlayerMap.Model;

namespace PlayerMap
{
    public class NewPlayerIdMap
    {
        public static void Map()
        {
            var players = new DataImporter<Player, MySqlPlayerMap>().LoadData(@"Data\\tblplayers2.csv")
                .ToDictionary(x => x.PlayerIid);
            var result = new List<MasterPlayer>();
            foreach (var player in players)
            {
                
            }
        }
    }
}