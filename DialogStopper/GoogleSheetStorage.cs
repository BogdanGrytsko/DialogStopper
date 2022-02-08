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
        
        public async Task UploadData(string logFile)
        {
            var lines = await File.ReadAllLinesAsync(logFile);

            var range = $"{meditationSheet}!A:G";
            var valueRange = new ValueRange {Values = new List<IList<object>>()};
            valueRange.Values.Add(new List<object> { "TimeStamp", "Count", "Min", "Max", "Avg", "Std", "Entries" });
            foreach (var line in lines)
            {
                var (dateTime, values) = LogTransformer.GetEntry(line);
                var avgSpan = values.Length > 1 ? (double)values.Last() / (values.Length - 1) : 0;
                var rowValues = new List<object>
                {
                    dateTime, values.Length, string.Empty, string.Empty, Math.Round(avgSpan), string.Empty,
                    string.Join(";", values)
                };
                valueRange.Values.Add(rowValues);
            }

            var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, sheetId, range);
            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            await appendRequest.ExecuteAsync();
        }
        
        public async Task SyncAllHistory(string logFile)
        {
            var rangeOfExisting = $"{meditationSheet}!A1:K160";
            var request = sheetsService.Spreadsheets.Values.Get(sheetId, rangeOfExisting);
            var response = await request.ExecuteAsync();
            var values = response.Values;
        }
    }
}
