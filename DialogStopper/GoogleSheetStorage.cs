using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace DialogStopper
{
    public class GoogleSheetStorage<T>
    {
        private static readonly string[] scopes = { SheetsService.Scope.Spreadsheets };
        
        protected readonly string SheetId;
        protected readonly SheetsService SheetsService;

        public GoogleSheetStorage(string sheetId)
        {
            this.SheetId = sheetId;
            using var fs = File.OpenRead("api-keys.json");
            var credential = GoogleCredential.FromStream(fs).CreateScoped(scopes);
            SheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = nameof(MeditationGoogleSheetStorage) + "DialogStopper"
            });
        }

        protected virtual string SheetName => $"{typeof(T).Name}s";

        public async Task Add(List<T> data, bool addHeaders)
        {
            var range = $"{SheetName}!A:G";
            var valueRange = new ValueRange {Values = new List<IList<object>>()};
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (addHeaders)
            {
                var list = new List<object>(propertyInfos.Length);
                foreach (var propertyInfo in propertyInfos)
                {
                    list.Add(propertyInfo.Name);
                }
                valueRange.Values.Add(list);
            }

            foreach (var entry in data)
            {
                var rowValues = new List<object>(propertyInfos.Length);
                foreach (var prop in propertyInfos)
                {
                    var value = GetPropValue<T>(entry, prop.Name);
                    if (prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                    {
                        var prepared = ((IEnumerable)value).Cast<object>().Select(x => x.ToString());
                        rowValues.Add(string.Join(";", prepared));                        
                    }
                    else
                    {
                        rowValues.Add(value);                        
                    }
                }
                valueRange.Values.Add(rowValues);
            }

            var appendRequest = SheetsService.Spreadsheets.Values.Append(valueRange, SheetId, range);
            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            await appendRequest.ExecuteAsync();
        }
        
        public async Task Add(T entry)
        {
            await Add(new List<T> { entry }, false);
        }
        
        public static object GetPropValue<T>(object src, string propName)
        {
            return typeof(T).GetProperty(propName).GetValue(src, null);
        }
    }
}