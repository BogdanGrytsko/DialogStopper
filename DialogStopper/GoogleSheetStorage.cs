using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;

namespace DialogStopper
{
    public class GoogleSheetStorage
    {
        public static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string sheetId = "1o1Yi_xWMEcN0ki5zy6Yk_NsYOCBC52ix9t57T_ulMn8";
        private readonly string dreamsSheet = "Dreams";
        private readonly string meditationSheet = "Meditation";
        private readonly SheetsService sheetsService;

        public GoogleSheetStorage()
        {
            using var fs = File.OpenRead("api-keys.json");
            var credential = GoogleCredential.FromStream(fs).CreateScoped(Scopes);
            sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = nameof(GoogleSheetStorage) + "DialogStopper"
            });
        }
        
        public async Task AddFromFile(string logFile)
        {
            var lines = await File.ReadAllLinesAsync(logFile);
            var entries = lines.Select(MeditationEntry.FromTxtFile).ToList();
            await Add(entries, true);
        }

        public async Task Add(List<MeditationEntry> meditationEntries, bool addHeaders)
        {
            var range = $"{meditationSheet}!A:G";
            var valueRange = new ValueRange {Values = new List<IList<object>>()};
            if (addHeaders)
            {
                valueRange.Values.Add(new List<object>
                {
                    nameof(MeditationEntry.TimeStamp), nameof(MeditationEntry.Count), nameof(MeditationEntry.Avg),
                    nameof(MeditationEntry.Min), nameof(MeditationEntry.Max), nameof(MeditationEntry.Std),
                    nameof(MeditationEntry.Points)
                });
            }

            foreach (var entry in meditationEntries)
            {
                var rowValues = new List<object>
                {
                    entry.TimeStamp, entry.Count, entry.Avg, entry.Min, entry.Max, entry.Std,
                    string.Join(";", entry.Points)
                };
                valueRange.Values.Add(rowValues);
            }

            var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, sheetId, range);
            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            await appendRequest.ExecuteAsync();
        }

        public async Task UpdateStats()
        {
            var rangeOfExisting = $"{meditationSheet}!A1:G";
            var request = sheetsService.Spreadsheets.Values.Get(sheetId, rangeOfExisting);
            var response = await request.ExecuteAsync();
            var values = response.Values;
            var map = GetHeaderMap(values.First());

            var updateRange = $"{meditationSheet}!C2:F{values.Count}";
            var lists = new List<IList<object>>();
            foreach (var line in values.Skip(1))
            {
                var pointsStr =  ((string)line[map[nameof(MeditationEntry.Points)]]).Split(";", StringSplitOptions.RemoveEmptyEntries);
                var points = pointsStr.Select(long.Parse).ToList();
                var entry = new MeditationEntry(DateTime.MinValue, points);
                
                lists.Add(new List<object> {Math.Round(entry.Avg), entry.Min, entry.Max, Math.Round(entry.Std)});
            }

            var valueRange = new ValueRange { Values = lists };
            var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, sheetId, updateRange);
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

        public async Task Add(MeditationEntry meditationEntry)
        {
            await Add(new List<MeditationEntry> { meditationEntry }, false);
        }
    }
}
