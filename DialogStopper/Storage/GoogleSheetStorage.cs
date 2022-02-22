using System;
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

namespace DialogStopper.Storage
{
    public class GoogleSheetStorage<T> where T : new()
    {
        private const string listSeparator = ";";
        private static readonly string[] scopes = { SheetsService.Scope.Spreadsheets };
        
        protected readonly string SheetId;
        protected readonly SheetsService SheetsService;
        protected readonly PropertyInfo[] PropertyInfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public GoogleSheetStorage(string sheetId)
        {
            SheetId = sheetId;
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

        public async Task Add(List<T> data, bool addHeaders)
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
                    else
                    {
                        rowValues.Add(value);                        
                    }
                }
                valueRange.Values.Add(rowValues);
            }

            var appendRequest = SheetsService.Spreadsheets.Values.Append(valueRange, SheetId, GetRange());
            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            await appendRequest.ExecuteAsync();
        }

        private static bool IsEnumerable(PropertyInfo prop)
        {
            return prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType);
        }

        public async Task Add(T entry)
        {
            await Add(new List<T> { entry }, false);
        }

        protected string GetRange(int? startRow = null, int? endRow = null)
        {
            return $"{SheetName}!A{startRow}:{GetEndLetter()}{endRow}";
        }

        private string GetEndLetter()
        {
            if (PropertyInfos.Length > 26)
                throw new Exception("Not supported yet");
            return ((char)('A' - 1 + PropertyInfos.Length)).ToString();
        }
        
        public async Task<List<T>> Get(int? endRow = null)
        {
            var request = SheetsService.Spreadsheets.Values.Get(SheetId, GetRange(1, endRow));
            var response = await request.ExecuteAsync();
            var values = response.Values;
            
            var map = GetHeaderMap(values.First());
            var result = new List<T>();
            foreach (var valueList in values.Skip(1))
            {
                var data = new T();
                foreach (var prop in PropertyInfos)
                {
                    var value = valueList[map[prop.Name]];
                    var type = prop.PropertyType;
                    if (!IsEnumerable(prop))
                    {
                        var converted = Convert.ChangeType(value, type);
                        SetPropValue(data, prop, converted);    
                    }
                    else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList<>) 
                        || type.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        Type itemType = type.GetGenericArguments()[0];
                        var split = ((string)value).Split(listSeparator, StringSplitOptions.RemoveEmptyEntries);
                        var listType = typeof(List<>);
                        var constructedListType = listType.MakeGenericType(itemType);
                        var instance = (IList)Activator.CreateInstance(constructedListType);

                        foreach (var s in split)
                        {
                            var x = Convert.ChangeType(s, itemType);
                            instance.Add(x);
                        }
                        SetPropValue(data, prop, instance);
                    }
                }
                result.Add(data);
            }
            return result;
        }
        
        protected static Dictionary<string, int> GetHeaderMap(IList<object> headers)
        {
            var headerMap = new Dictionary<string, int>();
            for (int i = 0; i < headers.Count; i++)
            {
                headerMap.Add(headers[i].ToString(), i);
            }

            return headerMap;
        }
        
        public static object GetPropValue(object src, PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(src, null);
        }

        public static void SetPropValue(object src, PropertyInfo propertyInfo, object value)
        {
            if (propertyInfo.CanWrite)
                propertyInfo.SetValue(src, value);
        }

        public async Task Delete(int startRow, int? endRow = null)
        {
            var requestBody = new ClearValuesRequest();
            var deleteRequest =
                SheetsService.Spreadsheets.Values.Clear(requestBody, SheetId, GetRange(startRow, endRow ?? startRow));
            await deleteRequest.ExecuteAsync();
        }

        public async Task Update(List<T> data)
        {
            //todo: get by range, headers needed?
            var sheetData = Get();
            
            var lists = new List<IList<object>>();
            
            var valueRange = new ValueRange { Values = lists };
            //todo : fix
            var updateRequest = SheetsService.Spreadsheets.Values.Update(valueRange, SheetId, GetRange(-1, -1));
            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await updateRequest.ExecuteAsync();
        }
    }
}