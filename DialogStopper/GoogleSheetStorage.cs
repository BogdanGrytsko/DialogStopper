using Google.Apis.Sheets.v4;
using System;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace DialogStopper
{
    public class GoogleSheetStorage
    {
        public static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string sheetId = "1o1Yi_xWMEcN0ki5zy6Yk_NsYOCBC52ix9t57T_ulMn8";
        //todo : second tab for meditation
        private readonly string sheetName = "Dreams";
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
        
        public async Task SyncAllHistory()
        {
            var rangeOfExisting = $"{sheetName}!A1:K160";
            var request = sheetsService.Spreadsheets.Values.Get(sheetId, rangeOfExisting);
            var response = await request.ExecuteAsync();
            var values = response.Values;
        }
    }
}
