using DialogStopper.Storage;
using PlayerMap.BasketballReference.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMap.BasketballReference
{
    public class MapNBAToCollege
    {
        public async Task Map()
        {
            var playerCareersNBA = await GetPlayerCareersNBA(@"C:\temp\Sportradar\PlayerCareersNBA.csv");
            var playerCareersCMRef = await GetPlayerCareersCMRef(@"C:\temp\Sportradar\PlayerCareersCMRef.csv");
            var mapNBAToColleges = new List<PlayerCareer>();
            var playerCareersNBAGroup = playerCareersNBA.GroupBy(x => x.BBRefPlayer.Name).ToList();
            foreach (var playerCareer in playerCareersNBAGroup)
            {
                if (playerCareersCMRef.Contains(playerCareer.Key))
                    playerCareersNBA.AddRange(playerCareersCMRef[playerCareer.Key].ToList());
            }
            var m = playerCareersNBA.GroupBy(x => x.BBRefPlayer.Name).ToList();
            foreach (var playerCareer in m)
            {
                mapNBAToColleges.AddRange(playerCareer);
            }
            DataExporter.Export(mapNBAToColleges, @"C:\temp\Sportradar\MapNBAToCollege.csv");
        }
        private static async Task<List<PlayerCareer>> GetPlayerCareersNBA(string path)
        {
            var playerCareers = DataExporter.Import(path);
            return new DataImporter<PlayerCareer, PlayerCareerMap>().LoadData(playerCareers, ";");
        }
        private static async Task<Lookup<string, PlayerCareer>> GetPlayerCareersCMRef(string path)
        {
            var playerCareers = DataExporter.Import(path);
            return (Lookup<string, PlayerCareer>)new DataImporter<PlayerCareer, PlayerCareerMap>().LoadData(playerCareers, ";")
                .Where(a => a.BBRefPlayer.IsNBA).ToLookup(a => a.BBRefPlayer.Name, a => a);
        }
    }
}

