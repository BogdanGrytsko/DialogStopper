using System;
using System.Linq;
using HtmlAgilityPack;
using PlayerMap.BasketballReference.Model;

namespace PlayerMap.BasketballReference
{
    public class RoasterTableParser
    {
        public static BBRefPlayer GetPlayer(HtmlNode row)
        {
            var number = row.SelectNodes("th[@data-stat='number']")?.FirstOrDefault()?.InnerText;
            var player = row.SelectNodes(ColumnA("player")).First();
            var college = row.SelectNodes(ColumnA("college"))?.FirstOrDefault();
            var bbRefPlayer = new BBRefPlayer
            {
                Name = player.InnerText,
                Url = player.Attributes["href"].Value,
                College = college?.InnerText,
                CollegeUrl = college?.Attributes["href"].Value,
                Pos = GetText(row, "pos"),
                Height = GetText(row, "height"),
                Weight = GetText(row, "weight"),
                BirthCountry = GetText(row, "birth_country"),
                YearsOfExpirience = GetText(row, "years_experience")
            };
            if (int.TryParse(number, out var numberInt))
                bbRefPlayer.Number = numberInt;
            if (DateTime.TryParse(GetText(row, "birth_date"), out var birthDate))
                bbRefPlayer.BirthDate = birthDate;

            return bbRefPlayer;
        }

        private static string GetText(HtmlNode row, string name)
        {
            return row.SelectNodes(Column(name))?.FirstOrDefault()?.InnerText;
        }

        private static string ColumnA(string name)
        {
            return $"{Column(name)}//a";
        }
        
        private static string Column(string name)
        {
            return $"td[@data-stat='{name}']";
        }
    }
}