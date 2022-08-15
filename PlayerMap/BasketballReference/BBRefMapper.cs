using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DialogStopper.Storage;
using PlayerMap.BasketballReference.Model;

namespace PlayerMap.BasketballReference
{
    public class BBRefMapper
    {
        private const string NBA = "54457dce300969b132fcfb34", WNBA = "54457dce300969b132fcfb35";

        public async Task Map(string scrapedPlayersPath, string mongoPlayersPath, string boxScoresPath, string fileName)
        {
            var scrapedPlayers = new DataImporter<BBRefPlayer, BBRefPlayerMap>().LoadData(scrapedPlayersPath, ";").ToList();
            var mongoPlayers = new DataImporter<MongoPlayerDto, MongoPlayerDtoMap>().LoadData(mongoPlayersPath, ";").ToList();
            var boxScores = new DataImporter<BoxScoreDto, BoxScoreDtoMap>().LoadData(boxScoresPath, ";").ToList();

            var playerMap = GetMongoPlayerMap(mongoPlayers);
            var boxScoreMap = GetBoxScoreMap(boxScores);

            var playerCareers = new List<PlayerCareer>();
            foreach (var group in scrapedPlayers.GroupBy(x => x.Url))
            {
                foreach (var bbRefPlayer in group)
                {
                    var pc = new PlayerCareer { BBRefPlayer = bbRefPlayer };
                    playerMap.TryGetValue(bbRefPlayer.MongoTeamId, out var possiblePlayers);
                    boxScoreMap.TryGetValue($"{bbRefPlayer.MongoSeasonId}_{bbRefPlayer.MongoTeamId}", out var possibleBoxScores);
                    if (possibleBoxScores != null)
                    {
                        new BBRefPlayerMapper(pc, possiblePlayers, possibleBoxScores).ApplyMap();
                        playerCareers.Add(pc);
                    }
                }
            }

            DataExporter.Export(playerCareers, @$"C:\temp\Sportradar\{fileName}.csv");
        }

        private static Dictionary<string, List<MongoPlayerDto>> GetMongoPlayerMap(List<MongoPlayerDto> leaguePlayers)
        {
            return leaguePlayers.GroupBy(x => x.MongoTeamId)
                .ToDictionary(x => x.Key, x => x.ToList());
        }
        
        private static Dictionary<string, List<BoxScoreDto>> GetBoxScoreMap(List<BoxScoreDto> boxScores)
        {
            return boxScores.GroupBy(x => $"{x.MongoSeasonId}_{x.MongoTeamId}")
                .ToDictionary(x => x.Key, x => x.DistinctBy(y => y.MongoPlayerId).ToList());
        }
    }
}