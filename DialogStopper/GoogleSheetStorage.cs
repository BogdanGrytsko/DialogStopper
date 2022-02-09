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
            await UploadData(lines);
        }

        public async Task UploadData(string[] lines)
        {
            var range = $"{meditationSheet}!A:G";
            var valueRange = new ValueRange {Values = new List<IList<object>>()};
            valueRange.Values.Add(new List<object> { nameof(MeditationEntry.TimeStamp), "Count", "Avg", "Min", "Max", "Std", nameof(MeditationEntry.Points) });
            foreach (var line in lines)
            {
                var (dateTime, values) = LogTransformer.GetEntry(line);
                var rowValues = new List<object>
                {
                    dateTime, values.Length, string.Empty, string.Empty, string.Empty, string.Empty,
                    string.Join(";", values)
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
                var points = pointsStr.Select(int.Parse).ToList();
                var segments = GetSegments(points);
                var avg = Math.Round(segments.Average());
                var min = segments.Min();
                var max = segments.Max();
                var std = Math.Round(StandardDeviation(segments));
                
                lists.Add(new List<object> {avg, min, max, std});
            }

            var valueRange = new ValueRange { Values = lists };
            var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, sheetId, updateRange);
            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await updateRequest.ExecuteAsync();
        }
        
        private static double StandardDeviation(IList<int> sequence)
        {
            if (!sequence.Any()) return 0;
            
            var average = sequence.Average();
            var sum = sequence.Sum(d => Math.Pow(d - average, 2));
            var result = Math.Sqrt(sum / (sequence.Count - 1));
            return result;
        }

        private List<int> GetSegments(List<int> points)
        {
            var segments = new List<int>();
            var prev = 0; 
            foreach (var p in points)
            {
                var segment = p - prev;
                segments.Add(segment);
                prev = p;
            }
            return segments;
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
