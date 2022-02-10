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

namespace DialogStopper
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
            //todo : A:G shouldn't be hardcoded
            var range = $"{SheetName}!A:G";
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
                    //todo fix
                    var value = GetPropValue(entry, prop.Name);
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

            var appendRequest = SheetsService.Spreadsheets.Values.Append(valueRange, SheetId, range);
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

        public async Task<List<T>> Get(int? endRow = null)
        {
            //todo: fix A G
            var rangeOfExisting = $"{SheetName}!A{1}:G{endRow}";
            var request = SheetsService.Spreadsheets.Values.Get(SheetId, rangeOfExisting);
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
        
        public static object GetPropValue(object src, string propName)
        {
            return typeof(T).GetProperty(propName).GetValue(src, null);
        }

        public static void SetPropValue(object src, PropertyInfo propertyInfo, object value)
        {
            if (propertyInfo.CanWrite)
                propertyInfo.SetValue(src, value);
        }
    }
}