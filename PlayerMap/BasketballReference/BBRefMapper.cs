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
        private const int MinRating = 95;
        
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
            if (pc.BBRefPlayer.Url == "/players/w/welscji01.html")
            {
                
            }
            if (!possiblePlayers.Any())
            {
                pc.Comment = "No players found for given league+season+team";
                return;
            }

            var fuzzRated = GetFuzzRating(possiblePlayers, pc.BBRefPlayer).OrderByDescending(x => x.Item2).ToList();
            var (mongoPlayerId, rating) = GetPlayerId(fuzzRated, pc.BBRefPlayer.Number, out var comment);
            pc.Comment = comment;
            pc.MongoPlayerId = mongoPlayerId;
            pc.NameRating = rating;
        }

        private static (string, int) GetPlayerId(List<(MongoPlayerDto player, int rating)> fuzzRated, int number, out string comment)
        {
            var byName = fuzzRated.Where(x => x.Item2 >= MinRating).ToList();
            if (byName.Any())
            {
                var byNameAndNumber = byName.Where(x => x.player.Number == number).ToList();
                if (byNameAndNumber.Count == 1)
                {
                    comment = "Strong match by both name and number";
                    return GetRatedPlayer(byNameAndNumber);
                }
                if (byNameAndNumber.Count > 1)
                {
                    comment = "Multi match by both name and number. Use events to refine";
                    return GetRatedPlayer(byNameAndNumber);
                }
                //only name
                if (byName.Count == 1)
                {
                    comment = "Match only by name";
                    return GetRatedPlayer(byName);
                }
                else
                {
                    comment = "Multi match only by name. Use events to refine";     
                    return GetRatedPlayer(byName);
                }
            }

            comment = "No players found by name";
            return (null, 0);
        }

        private static (string, int) GetRatedPlayer(List<(MongoPlayerDto player, int rating)> players)
        {
            return (string.Join(";", players.Select(x => x.player.Id).ToArray()), players.First().rating);
        }

        private static IEnumerable<(MongoPlayerDto, int)> GetFuzzRating(List<MongoPlayerDto> possiblePlayers, BBRefPlayer bbRefPlayer)
        {
            foreach (var possiblePlayer in possiblePlayers)
            {
                var fuzzRatio = Fuzz.Ratio(PrepareName(possiblePlayer.Name), PrepareName(bbRefPlayer.Name));
                yield return (possiblePlayer, fuzzRatio);
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