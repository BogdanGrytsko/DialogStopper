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
        private const int WeakRating = 80;
        
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
                pc.MongoPlayer = new RatedMongoPlayer { Comment = "No players found for given league+season+team" };
                return;
            }

            var fuzzRated = GetFuzzRating(possiblePlayers, pc.BBRefPlayer).OrderByDescending(x => x.Item2).ToList();
            var ratedPlayer = GetRatedPlayer(fuzzRated, pc.BBRefPlayer.Number, MinRating);
            if (ratedPlayer.MongoPlayerId == null)
            {
                var weakRated = fuzzRated.Where(x => x.Item2 >= WeakRating).Take(1).ToList();
                ratedPlayer = GetRatedPlayer(weakRated, pc.BBRefPlayer.Number, WeakRating, "Weak ");
            }
            pc.MongoPlayer = ratedPlayer;
        }

        private static RatedMongoPlayer GetRatedPlayer(List<(MongoPlayerDto player, int rating)> fuzzRated, int number, int minRating, string prefix = null)
        {
            var byName = fuzzRated.Where(x => x.Item2 >= minRating).ToList();
            if (byName.Any())
            {
                var byNameAndNumber = byName.Where(x => x.player.Number == number).ToList();
                if (byNameAndNumber.Count >= 1)
                {
                    return RatePlayer(byNameAndNumber, $"{prefix}Match by name and number");
                }
                if (byName.Count >= 1)
                {
                    return RatePlayer(byName, $"{prefix}Match by name only");
                }
            }

            return new RatedMongoPlayer { Comment = "No players found by name"};
        }

        private static RatedMongoPlayer RatePlayer(List<(MongoPlayerDto player, int rating)> players, string comment)
        {
            if (players.Count > 1)
                comment = $"Multi {comment}. Use events to refine";
            return GetRatedPlayer(players, comment);
        }

        private static RatedMongoPlayer GetRatedPlayer(List<(MongoPlayerDto player, int rating)> players, string comment)
        {
            return new RatedMongoPlayer
            {
                MongoPlayerId = string.Join(";", players.Select(x => x.player.Id).ToArray()),
                Comment = comment,
                Rating = players.First().rating
            };
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