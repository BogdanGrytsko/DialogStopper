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
            var data = await GetAsync();
            var set = data.Select(x => x.TimeStamp).ToHashSet();
            entries = entries.Where(x => !set.Contains(x.TimeStamp)).ToList();
            await AddAsync(entries, !data.Any());
        }

        public async Task UpdateStats(int startRow)
        {
            var data = await GetAsync(startRow);
            var updateRange = $"{SheetName}!C{startRow}:I{startRow + data.Count - 1}";
            var lists = new List<IList<object>>();
            foreach (var line in data)
            {
                var entry = new Meditation(DateTime.MinValue, line.Points);
                lists.Add(new List<object> {Math.Round(entry.Avg), entry.Min, entry.Max, Math.Round(entry.Std), entry.Sum, entry.Total, entry.Percent});
            }

            var valueRange = new ValueRange { Values = lists };
            var updateRequest = SheetsService.Spreadsheets.Values.Update(valueRange, sheetId, updateRange);
            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await updateRequest.ExecuteAsync();
        }

        protected override object ParseListObject(string s, Type itemType)
        {
            var data = s.Replace("(", string.Empty).Replace(")", string.Empty);
            var vals = data.Split(",");
            Enum.TryParse(vals[1], out PointType pointType);
            return (long.Parse(vals[0]), pointType);
        }
    }
}
