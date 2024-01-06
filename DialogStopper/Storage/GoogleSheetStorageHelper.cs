using System.Collections;
using Google.Apis.Sheets.v4;
using System.Reflection;

namespace DialogStopper.Storage;

public class GoogleSheetStorageHelper
{
    public static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

    public static void AddValue(object value, List<object> rowValues)
    {
        switch (value)
        {
            case decimal decimalVal:
                rowValues.Add(decimalVal.ToString("F2"));
                break;
            case DateTime dateTimeVal:
                rowValues.Add(dateTimeVal.ToString("G"));
                break;
            case Enum enumVal:
                rowValues.Add(enumVal.ToString());
                break;
            default:
                rowValues.Add(value);
                break;
        }
    }

    public static bool IsEnumerable(PropertyInfo prop)
    {
        return prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType);
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

    public static object GetConvertedValue(object value, Type type)
    {
        if (value is string && string.IsNullOrEmpty(value.ToString()))
            return GetDefault(type);
        return Convert.ChangeType(value, type);
    }

    public static object GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}