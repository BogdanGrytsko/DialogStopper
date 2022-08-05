using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DialogStopper.Storage;
using FuzzySharp;
using PlayerMap.BasketballReference.Model;

namespace PlayerMap.BasketballReference
{
    public class BBRefMapper
    {
        private const string NBA = "54457dce300969b132fcfb34", WNBA = "54457dce300969b132fcfb35";
        
        public async Task Map()
        {
            var scrapedPlayers = new DataImporter<BBRefPlayer, BBRefPlayerMap>().LoadData(@"C:\temp\Sportradar\BBRefPlayers.csv", ";").ToList();
            var mongoPlayers = new DataImporter<MongoPlayerDto, MongoPlayerDtoMap>().LoadData(@"C:\temp\Sportradar\AllPlayers.csv", ";").ToList();
            var playerMap = GetMongoPlayerMap(mongoPlayers);
            
            var playerCareers = new List<PlayerCareer>();
            foreach (var group in scrapedPlayers.GroupBy(x => x.Url))
            {
                foreach (var bbRefPlayer in group)
                {
                    var pc = new PlayerCareer { BBRefPlayer = bbRefPlayer };
                    playerMap.TryGetValue(GetMongoKey(bbRefPlayer), out var possiblePlayers);
                    Map(pc, possiblePlayers);
                    playerCareers.Add(pc);
                }
            }
            
            DataExporter.Export(playerCareers, @"C:\temp\Sportradar\PlayerCareers.csv");
        }

        private static void Map(PlayerCareer pc, List<MongoPlayerDto> possiblePlayers)
        {
            if (!possiblePlayers.Any())
            {
                pc.Comment = "No players found for given league+season+team";
                return;
            }

            //algo change : team + name. If number = single = good match. Flag if multi. If 
            var byName = GetByNameMatch(possiblePlayers, pc.BBRefPlayer).ToList();
            var mongoPlayerId = GetPlayerId(byName, pc.BBRefPlayer.Number, out var comment);
            pc.Comment = comment;
            pc.MongoPlayerId = mongoPlayerId;
        }

        private static string GetPlayerId(List<MongoPlayerDto> byName, int number, out string comment)
        {
            if (byName.Any())
            {
                var byNameAndNumber = byName.Where(x => x.Number == number).ToList();
                if (byNameAndNumber.Count == 1)
                {
                    comment = "Strong match by both name and number";
                    return byNameAndNumber.Single().Id;
                }
                if (byNameAndNumber.Count > 1)
                {
                    comment = "Multi match by both name and number. Use events to refine";
                    return string.Join(";", byNameAndNumber.Select(x => x.Id).ToArray());
                }
                //only name
                if (byName.Count == 1)
                {
                    comment = "Match only by name";
                    return byName.Single().Id;
                }
                else
                {
                    comment = "Multi match only by name. Use events to refine";     
                    return string.Join(";", byName.Select(x => x.Id).ToArray());
                }
            }

            comment = "No players found by name";
            return null;
        }

        private static IEnumerable<MongoPlayerDto> GetByNameMatch(List<MongoPlayerDto> possiblePlayers, BBRefPlayer bbRefPlayer)
        {
            foreach (var possiblePlayer in possiblePlayers)
            {
                if (Fuzz.Ratio(PrepareName(possiblePlayer.Name), PrepareName(bbRefPlayer.Name)) >= 95)
                    yield return possiblePlayer;
            }
        }

        private static string PrepareName(string name)
        {
            return name.ToUpper();
        }

        private static string GetMongoKey(BBRefPlayer player)
        {
            return player.MongoTeamId;
        }

        private static Dictionary<string, List<MongoPlayerDto>> GetMongoPlayerMap(List<MongoPlayerDto> leaguePlayers)
        {
            return leaguePlayers.GroupBy(x => x.MongoTeamId)
                .ToDictionary(x => x.Key, x => x.ToList());
        }
    }
}