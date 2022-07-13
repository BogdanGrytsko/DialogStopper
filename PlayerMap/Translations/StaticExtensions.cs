using System.Globalization;

namespace PlayerMap.Translations
{
    public static class StaticExtensions
    {
        public static string ToPascalCase(this string str)
        {
            var info = CultureInfo.CurrentCulture.TextInfo;
            return info.ToTitleCase(str).Replace(" ", string.Empty);
        }
    }
}