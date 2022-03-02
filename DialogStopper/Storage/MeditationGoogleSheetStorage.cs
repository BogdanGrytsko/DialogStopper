using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace DialogStopper.Storage
{
    public class MeditationGoogleSheetStorage : GoogleSheetStorage<Meditation>
    {
        private static string sheetId = "1o1Yi_xWMEcN0ki5zy6Yk_NsYOCBC52ix9t57T_ulMn8";

        public MeditationGoogleSheetStorage() : base(sheetId)
        {
        }

        public async Task AddFromFile(string logFile)
        {
            var lines = await File.ReadAllLinesAsync(logFile);
            var entries = lines.Select(Meditation.FromTxtFile).ToList();
            //only new
            var data = await Get();
            var set = data.Select(x => x.TimeStamp).ToHashSet();
            entries = entries.Where(x => !set.Contains(x.TimeStamp)).ToList();
            await Add(entries, !data.Any());
        }

        public async Task UpdateStats()
        {
            var data = await Get();

            var updateRange = $"{SheetName}!C2:F{data.Count + 1}";
            var lists = new List<IList<object>>();
            foreach (var line in data)
            {
                var entry = new Meditation(DateTime.MinValue, line.Points);
                lists.Add(new List<object> {Math.Round(entry.Avg), entry.Min, entry.Max, Math.Round(entry.Std)});
            }

            var valueRange = new ValueRange { Values = lists };
            var updateRequest = SheetsService.Spreadsheets.Values.Update(valueRange, sheetId, updateRange);
            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await updateRequest.ExecuteAsync();
        }
    }
}
