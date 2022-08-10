using System;
using System.Linq;
using HtmlAgilityPack;
using PlayerMap.BasketballReference.Model;

namespace PlayerMap.BasketballReference.Scrape
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
                YearsOfExperience = GetText(row, "years_experience")
            };
            if (int.TryParse(number, out var numberInt))
                bbRefPlayer.Number = numberInt;
            if (DateTime.TryParse(GetText(row, "birth_date"), out var birthDate))
                bbRefPlayer.BirthDate = birthDate;

            return bbRefPlayer;
        }

        public static BBRefPlayer GetSRefPlayer(HtmlNode row)
        {
            var number = row.SelectNodes("td[@data-stat='number']")?.FirstOrDefault()?.InnerText;
            var player = row.SelectNodes("th[@data-stat='player']")?.FirstOrDefault();
            var college = row.SelectNodes(ColumnA("high_school"))?.FirstOrDefault();
            var bbRefPlayer = new BBRefPlayer()
            {
               Name = player?.InnerText,
               Url = player?.SelectSingleNode(".//a").Attributes["href"].Value,
               College = college?.InnerText,
               CollegeUrl = college?.Attributes["href"].Value,
               Pos = GetText(row, "pos"),
               Height = GetText(row, "height"),
               Weight = GetText(row, "weight"),
               BirthCountry = GetText(row, "hometown"),
               RSCITop100 = GetText(row, "rsci"),
               Summary = GetText(row, "summary"),
               Class = GetText(row, "class"),
            };
            if (int.TryParse(number, out var numberInt))
                bbRefPlayer.Number = numberInt;
            if (DateTime.TryParse(GetText(row, "birth_date"), out var birthDate))
                bbRefPlayer.BirthDate = birthDate;
            return bbRefPlayer;
        }

        public static string GetText(HtmlNode row, string name)
        {
            return row.SelectNodes(Column(name))?.FirstOrDefault()?.InnerText;
        }

        public static string ColumnA(string name)
        {
            return $"{Column(name)}//a";
        }
        
        public static string Column(string name)
        {
            return $"td[@data-stat='{name}']";
        }
    }
}