using System.Collections;
using System.Reflection;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace DialogStopper.Storage
{
    public class GoogleSheetStorage<T> where T : new()
    {
        private const string ListSeparator = ";";
        private readonly string _sheetId;
        public SheetsService SheetsService { get; }
        protected readonly PropertyInfo[] PropertyInfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public GoogleSheetStorage(string sheetId)
        {
            this._sheetId = sheetId;
            using var fs = File.OpenRead("api-keys.json");
            var credential = GoogleCredential.FromStream(fs).CreateScoped(GoogleSheetStorageHelper.Scopes);
            SheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = nameof(MeditationGoogleSheetStorage) + "DialogStopper"
            });
            SheetName = $"{typeof(T).Name}s";
        }

        public string SheetName { protected get; set; }

        public async Task AddAsync(List<T> data, bool addHeaders)
        {
            var valueRange = new ValueRange {Values = new List<IList<object>>()};
            if (addHeaders)
            {
                var list = new List<object>(PropertyInfos.Length);
                foreach (var propertyInfo in PropertyInfos)
                {
                    list.Add(propertyInfo.Name);
                }
                valueRange.Values.Add(list);
            }

            foreach (var entry in data)
            {
                var rowValues = new List<object>(PropertyInfos.Length);
                foreach (var prop in PropertyInfos)
                {
                    var value = GoogleSheetStorageHelper.GetPropValue(entry, prop);
                    if (GoogleSheetStorageHelper.IsEnumerable(prop))
                    {
                        var prepared = ((IEnumerable)value).Cast<object>().Select(x => x.ToString());
                        rowValues.Add(string.Join(ListSeparator, prepared));                        
                    }
                    else
                    {
                        GoogleSheetStorageHelper.AddValue(value, rowValues);
                    }
                }
                valueRange.Values.Add(rowValues);
            }

            await Append(valueRange);
        }

        public async Task Append(ValueRange valueRange)
        {
            var appendRequest = SheetsService.Spreadsheets.Values.Append(valueRange, _sheetId, GetRange());
            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var _ = await appendRequest.ExecuteAsync();
        }

        public async Task AddAsync(T entry)
        {
            await AddAsync(new List<T> { entry }, false);
        }

        private string GetRange(int? startRow = null, int? endRow = null)
        {
            return GetRange(PropertyInfos.Length, startRow, endRow);
        }

        public string GetRange(int headerCount, int? startRow = null, int? endRow = null)
        {
            return $"{SheetName}!A{startRow}:{GetEndLetter(headerCount)}{endRow}";
        }

        private static string GetEndLetter(int length)
        {
            if (length > 26)
                throw new Exception("Not supported yet");
            return ((char)('A' - 1 + length)).ToString();
        }

        public async Task<List<T>> GetAsync(int startRow = 1, int? endRow = null)
        {
            var range = GetRange(startRow: startRow, endRow: endRow);
            var request = SheetsService.Spreadsheets.Values.Get(_sheetId, range);
            var response = await request.ExecuteAsync();
            var values = response.Values;
            
            var map = await GetHeaderMap(values.First(), startRow);
            var result = new List<T>();
            //skip only if header present : startRow == 1
            foreach (var valueList in startRow == 1 ? values.Skip(1) : values)
            {
                var data = new T();
                foreach (var prop in PropertyInfos)
                {
                    var idx = map[prop.Name];
                    // skip missing values
                    if (idx >= valueList.Count) continue;
                    var value = valueList[idx];
                    var type = prop.PropertyType;
                    if (!GoogleSheetStorageHelper.IsEnumerable(prop))
                    {
                        var converted = GoogleSheetStorageHelper.GetConvertedValue(value, type);
                        GoogleSheetStorageHelper.SetPropValue(data, prop, converted);
                    }
                    else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList<>)
                                                    || type.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        var itemType = type.GetGenericArguments()[0];
                        var split = ((string)value).Split(ListSeparator, StringSplitOptions.RemoveEmptyEntries);
                        var listType = typeof(List<>);
                        var constructedListType = listType.MakeGenericType(itemType);
                        var instance = (IList)Activator.CreateInstance(constructedListType);

                        foreach (var s in split)
                        {
                            var x = ParseListObject(s, itemType);
                            instance.Add(x);
                        }

                        GoogleSheetStorageHelper.SetPropValue(data, prop, instance);
                    }
                }

                result.Add(data);
            }

            return result;
        }

        protected virtual object ParseListObject(string s, Type itemType)
        {
            return Convert.ChangeType(s, itemType);
        }

        private async Task<Dictionary<string, int>> GetHeaderMap(IList<object> headers, int startRow)
        {
            var headerMap = new Dictionary<string, int>();
            if (startRow != 1)
            {
                var range = GetRange(1, 1);
                var request = SheetsService.Spreadsheets.Values.Get(_sheetId, range);
                var response = await request.ExecuteAsync();
                var values = response.Values;
                headers = values.First();
            }
            
            for (int i = 0; i < headers.Count; i++)
            {
                headerMap.Add(headers[i].ToString().Replace(" ", string.Empty), i);
            }

            return headerMap;
        }

        public Task DeleteAsync(int startRow = 1, int endRow = 1) 
        {
            return DeleteAsync(PropertyInfos.Length, startRow, endRow);
        }

        public async Task DeleteAsync(int headerCount, int startRow, int endRow)
        {
            var requestBody = new ClearValuesRequest();
            var deleteRequest =
                SheetsService.Spreadsheets.Values.Clear(requestBody, _sheetId, GetRange(headerCount, startRow, endRow));
            await deleteRequest.ExecuteAsync();
        }

        public async Task Update(List<T> data)
        {
            //todo: get by range, headers needed?
            var sheetData = GetAsync();
            
            var lists = new List<IList<object>>();
            
            var valueRange = new ValueRange { Values = lists };
            //todo : fix
            var updateRequest = SheetsService.Spreadsheets.Values.Update(valueRange, _sheetId, GetRange(-1, -1));
            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await updateRequest.ExecuteAsync();
        }
    }
}