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
        private const string listSeparator = ";";
        private static readonly string[] scopes = { SheetsService.Scope.Spreadsheets };

        private readonly string sheetId;
        protected readonly SheetsService SheetsService;
        protected readonly PropertyInfo[] PropertyInfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public GoogleSheetStorage(string sheetId)
        {
            this.sheetId = sheetId;
            using var fs = File.OpenRead("api-keys.json");
            var credential = GoogleCredential.FromStream(fs).CreateScoped(scopes);
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
                    var value = GetPropValue(entry, prop);
                    if (IsEnumerable(prop))
                    {
                        var prepared = ((IEnumerable)value).Cast<object>().Select(x => x.ToString());
                        rowValues.Add(string.Join(listSeparator, prepared));                        
                    }
                    else if (value is decimal decimalVal)
                    {
                        rowValues.Add(decimalVal.ToString("F2"));
                    }
                    else if (value is DateTime dateTimeVal)
                    {
                        rowValues.Add(dateTimeVal.ToString("G"));
                    }
                    else if (value is Enum enumVal)
                    {
                        rowValues.Add(enumVal.ToString());
                    }
                    else
                    {
                        rowValues.Add(value);                        
                    }
                }
                valueRange.Values.Add(rowValues);
            }

            var appendRequest = SheetsService.Spreadsheets.Values.Append(valueRange, sheetId, GetRange());
            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var _= await appendRequest.ExecuteAsync();
        }

        private static bool IsEnumerable(PropertyInfo prop)
        {
            return prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType);
        }

        public async Task AddAsync(T entry)
        {
            await AddAsync(new List<T> { entry }, false);
        }

        private string GetRange(int? startRow = null, int? endRow = null)
        {
            return $"{SheetName}!A{startRow}:{GetEndLetter()}{endRow}";
        }

        private string GetEndLetter()
        {
            if (PropertyInfos.Length > 26)
                throw new Exception("Not supported yet");
            return ((char)('A' - 1 + PropertyInfos.Length)).ToString();
        }

        public async Task<List<T>> GetAsync(int startRow = 1, int? endRow = null)
        {
            var range = GetRange(startRow, endRow);
            var request = SheetsService.Spreadsheets.Values.Get(sheetId, range);
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
                    if (!IsEnumerable(prop))
                    {
                        var converted = GetConvertedValue(value, type);
                        SetPropValue(data, prop, converted);
                    }
                    else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList<>)
                                                    || type.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        var itemType = type.GetGenericArguments()[0];
                        var split = ((string)value).Split(listSeparator, StringSplitOptions.RemoveEmptyEntries);
                        var listType = typeof(List<>);
                        var constructedListType = listType.MakeGenericType(itemType);
                        var instance = (IList)Activator.CreateInstance(constructedListType);

                        foreach (var s in split)
                        {
                            var x = ParseListObject(s, itemType);
                            instance.Add(x);
                        }

                        SetPropValue(data, prop, instance);
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

        private static object GetConvertedValue(object value, Type type)
        {
            if (value is string && string.IsNullOrEmpty(value.ToString()))
                return GetDefault(type);
            return Convert.ChangeType(value, type);
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        private async Task<Dictionary<string, int>> GetHeaderMap(IList<object> headers, int startRow)
        {
            var headerMap = new Dictionary<string, int>();
            if (startRow != 1)
            {
                var range = GetRange(1, 1);
                var request = SheetsService.Spreadsheets.Values.Get(sheetId, range);
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

        private static object GetPropValue(object src, PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(src, null);
        }

        private static void SetPropValue(object src, PropertyInfo propertyInfo, object value)
        {
            if (propertyInfo.CanWrite)
                propertyInfo.SetValue(src, value);
        }

        public async Task DeleteAsync(int startRow = 1, int? endRow = null)
        {
            var requestBody = new ClearValuesRequest();
            var deleteRequest =
                SheetsService.Spreadsheets.Values.Clear(requestBody, sheetId, GetRange(startRow, endRow ?? startRow));
            await deleteRequest.ExecuteAsync();
        }

        public async Task Update(List<T> data)
        {
            //todo: get by range, headers needed?
            var sheetData = GetAsync();
            
            var lists = new List<IList<object>>();
            
            var valueRange = new ValueRange { Values = lists };
            //todo : fix
            var updateRequest = SheetsService.Spreadsheets.Values.Update(valueRange, sheetId, GetRange(-1, -1));
            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await updateRequest.ExecuteAsync();
        }
    }
}