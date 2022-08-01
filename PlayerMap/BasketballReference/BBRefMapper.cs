using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DialogStopper.Storage;
using FuzzySharp;
using PlayerMap.BasketballReference.Model;
using PlayerMap.Model;

namespace PlayerMap.BasketballReference
{
    public class BBRefMapper
    {
        private const string NBA = "54457dce300969b132fcfb34", WNBA = "54457dce300969b132fcfb35";
        
        public async Task Map()
        {
            var scrapedPlayers = new DataImporter<BBRefPlayer, BBRefPlayerMap>().LoadData(@"C:\temp\Sportradar\BBRefPlayers.csv", ";").ToList();
            var mongoPlayers = new DataImporter<Player, MongoMasterPlayerMap>().LoadData(@"C:\temp\master.players.csv", ";").ToList();
            var leaguePlayers = mongoPlayers.Where(x => x.LeagueId == NBA || x.LeagueId == WNBA).ToList();
            leaguePlayers.ForEach(x => x.ParseTeam());
            var playerMap = GetMongoPlayerMap(leaguePlayers);
            //for Given League, Season, Number, PlayerName
            
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

        private static void Map(PlayerCareer pc, List<Player> possiblePlayers)
        {
            if (!possiblePlayers.Any())
            {
                pc.Comment = "No players found for given league+season+team";
                return;
            }

            var byNumber = possiblePlayers.FirstOrDefault(x => x.Number.HasValue && x.Number != 0 && x.Number == pc.BBRefPlayer.Number);
            var byName = GetByNameMatch(possiblePlayers, pc.BBRefPlayer).ToList();
            var mongoPlayerId = GetPlayerId(byNumber, byName, pc.BBRefPlayer.Number, out var comment);
            pc.Comment = comment;
            pc.MongoPlayerId = mongoPlayerId;
        }

        private static string GetPlayerId(Player byNumber, List<Player> byName, int number, out string comment)
        {
            if (byName.Count == 1 && byNumber != null)
            {
                if (byName.Single().Id == byNumber.Id || byName.Single().Number == number)
                {
                    comment = "Strong match by both name and number";
                    return byNumber.Id;
                }
                else
                {
                    comment = "Jersey number and name both found and don't match";
                    return null;
                }
            }
            else if (!byName.Any() && byNumber != null)
            {
                comment = "Selected by jersey number only";
                return byNumber.Id;
            }
            else if (byName.Any() && byNumber == null)
            {
                comment = "Selected by name only";
                if (byName.Count == 1)
                    return byName.Single().Id;
                else
                {
                    comment += ". Multiple players found in Mongo. Mapped to first";
                    return byName.First().Id;
                }
            }
            else if (!byName.Any() && byNumber == null)
            {
                comment = "No players found by name or jersey number";
                return null;
            }

            //we shouldn't get here
            comment = null;
            return null;
        }

        private static IEnumerable<Player> GetByNameMatch(List<Player> possiblePlayers, BBRefPlayer bbRefPlayer)
        {
            foreach (var possiblePlayer in possiblePlayers)
            {
                if (Fuzz.Ratio(possiblePlayer.Name, bbRefPlayer.Name) > 95)
                    yield return possiblePlayer;
            }
        }

        private static string GetMongoKey(BBRefPlayer player)
        {
            return $"{player.MongoSeasonId}_{player.MongoTeamId}";
        }

        private static Dictionary<string, List<Player>> GetMongoPlayerMap(List<Player> leaguePlayers)
        {
            return leaguePlayers.GroupBy(x => $"{x.SeasonId}_{x.TeamObj.MongoId}" ).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
}