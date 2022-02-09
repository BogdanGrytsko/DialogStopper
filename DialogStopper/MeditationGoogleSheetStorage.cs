using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;

namespace DialogStopper
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
            await Add(entries, true);
        }

        public async Task UpdateStats()
        {
            var rangeOfExisting = $"{SheetName}!A1:G";
            var request = SheetsService.Spreadsheets.Values.Get(sheetId, rangeOfExisting);
            var response = await request.ExecuteAsync();
            var values = response.Values;
            var map = GetHeaderMap(values.First());

            var updateRange = $"{SheetName}!C2:F{values.Count}";
            var lists = new List<IList<object>>();
            foreach (var line in values.Skip(1))
            {
                var pointsStr =  ((string)line[map[nameof(Meditation.Points)]]).Split(";", StringSplitOptions.RemoveEmptyEntries);
                var points = pointsStr.Select(long.Parse).ToList();
                var entry = new Meditation(DateTime.MinValue, points);
                
                lists.Add(new List<object> {Math.Round(entry.Avg), entry.Min, entry.Max, Math.Round(entry.Std)});
            }

            var valueRange = new ValueRange { Values = lists };
            var updateRequest = SheetsService.Spreadsheets.Values.Update(valueRange, sheetId, updateRange);
            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await updateRequest.ExecuteAsync();
        }
        
        private static Dictionary<string, int> GetHeaderMap(IList<object> headers)
        {
            var headerMap = new Dictionary<string, int>();
            for (int i = 0; i < headers.Count; i++)
            {
                headerMap.Add(headers[i].ToString(), i);
            }

            return headerMap;
        }
    }
}
