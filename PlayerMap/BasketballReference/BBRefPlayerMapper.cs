using System.Collections.Generic;
using System.Linq;
using FuzzySharp;
using PlayerMap.BasketballReference.Model;

namespace PlayerMap.BasketballReference
{
    public class BBRefPlayerMapper
    {
        private const int MinRating = 95;
        private const int WeakRating = 80;
        
        private readonly PlayerCareer pc;
        private readonly List<MongoPlayerDto> possiblePlayers;
        private readonly List<BoxScoreDto> possibleBoxScores;
        
        public BBRefPlayerMapper(PlayerCareer pc, List<MongoPlayerDto> possiblePlayers, List<BoxScoreDto> possibleBoxScores)
        {
            this.pc = pc;
            this.possiblePlayers = possiblePlayers;
            this.possibleBoxScores = possibleBoxScores;
        }

        public void ApplyMap()
        {
            if (!possiblePlayers.Any())
            {
                pc.MongoPlayer = new RatedMongoPlayer { Comment = "No players found for given league+season+team" };
                return;
            }

            var fuzzRated = GetFuzzRating(pc.BBRefPlayer).OrderByDescending(x => x.Item2).ToList();
            var ratedPlayer = GetRatedPlayer(fuzzRated, pc.BBRefPlayer, MinRating);
            if (ratedPlayer.MongoPlayerId == null)
            {
                var weakRated = fuzzRated.Where(x => x.Item2 >= WeakRating).Take(1).ToList();
                ratedPlayer = GetRatedPlayer(weakRated, pc.BBRefPlayer, WeakRating, "Weak ");
            }
            pc.MongoPlayer = ratedPlayer;
        }
        
        private IEnumerable<(MongoPlayerDto, int)> GetFuzzRating(BBRefPlayer bbRefPlayer)
        {
            foreach (var possiblePlayer in possiblePlayers)
            {
                var fuzzRatio = Fuzz.Ratio(PrepareName(possiblePlayer.Name), PrepareName(bbRefPlayer.Name));
                yield return (possiblePlayer, fuzzRatio);
            }
        }
        
        private RatedMongoPlayer GetRatedPlayer(List<(MongoPlayerDto player, int rating)> fuzzRated, BBRefPlayer bbRefPlayer, int minRating, string prefix = null)
        {
            var byName = fuzzRated.Where(x => x.Item2 >= minRating).ToList();
            if (byName.Any())
            {
                var byNameAndNumber = byName.Where(x => x.player.Number == bbRefPlayer.Number).ToList();
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

        private RatedMongoPlayer RatePlayer(List<(MongoPlayerDto player, int rating)> players, string comment)
        {
            if (players.Count > 1 && possibleBoxScores != null)
            {
                var playerIds = players.Select(x => x.player.Id).ToList();
                var boxScored = possibleBoxScores.Where(x => playerIds.Contains(x.MongoPlayerId)).ToList();
                if (boxScored.Count == 1)
                {
                    //results into 1 player
                    players = players.Where(x => x.player.Id == boxScored.Single().MongoPlayerId).ToList();
                    comment = $"{comment}. Refined with boxscores";
                }
            }
                
            return GetRatedPlayer(players, comment);
        }

        private static RatedMongoPlayer GetRatedPlayer(List<(MongoPlayerDto player, int rating)> players, string comment)
        {
            if (players.Count > 1)
                comment = $"Multi {comment}. Use events to refine";
            return new RatedMongoPlayer
            {
                MongoPlayerId = string.Join(";", players.Select(x => x.player.Id).ToArray()),
                Comment = comment,
                Rating = players.First().rating,
                SynergyPlayerName = players.First().player.Name,
            };
        }

        private static string PrepareName(string name)
        {
            return name.ToUpper();
        }
    }
}