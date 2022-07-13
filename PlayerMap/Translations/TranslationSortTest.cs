using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace PlayerMap.Translations
{
    public class TranslationSortTest
    {
        [Fact]
        public void SortTranslations()
        {
            var path = @"Translations\\Translations.csv";
            var lines = File.ReadAllLines(path);
            var result = lines.Skip(1).OrderBy(x => x.Split(";").First()).ToList();
            result = new List<string> { lines.First() }.Concat(result).ToList();
            File.WriteAllLines(@"Translations\\Translations_Sorted.csv", result, Encoding.UTF8);

            var template = File.ReadAllText(@"Translations\\template.txt");
            var languageMap = new Dictionary<int, string>();
            //language is the key
            var resultMap = new Dictionary<string, List<string>>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var words = line.Split(";");
                if (i == 0)
                {
                    for (int j = 0; j < words.Length; j++)
                    {
                        var language = words[j];
                        languageMap.Add(j, language);
                        resultMap.Add(language, new List<string>());
                    }
                    continue;
                }
                
                var baseWord = words.First().ToPascalCase();
                for (int j = 1; j < words.Length; j++)
                {
                    var word = words[j];
                    var replaced = template.Replace("{name}", baseWord).Replace("{value}", word);
                    resultMap[languageMap[j]].Add(replaced);
                }
            }

            foreach (var replaced in resultMap)
            {
                var concat = string.Join(Environment.NewLine, replaced.Value);
                File.WriteAllText(@"Translations\\" + replaced.Key + ".txt", concat);
            }
        }
    }
}