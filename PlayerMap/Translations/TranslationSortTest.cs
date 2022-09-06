using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace PlayerMap.Translations
{
    public class TranslationSortTest
    {
        [Fact]
        public void OutputTemplatedFiles()
        {
            var startIndex = 1;
            var shouldPascalCase = false;
            var inputPath = @"Translations\\CarbonThemeTranslations.csv";
            var updatePathTemplate =
                @"C:\Work\Repos\Ecovadis\EcovadisApp\Anakin.Spreadsheets\Resources\Cap\Client\ClientCorrectiveActionRecordResource.{0}.resx";
            var map = GenerateTemplatedTranslations(inputPath, startIndex, shouldPascalCase);
            SaveToFiles(map);
            PerformUpdate(map, updatePathTemplate);
        }

        private static void PerformUpdate(Dictionary<string,List<string>> map, string updatePathTemplate)
        {
            foreach (var entry in map)
            {
                var filePath = string.Format(updatePathTemplate, entry.Key.ToLower());
                if (entry.Key.ToLower() == "en")
                    filePath = filePath.Replace(".en", string.Empty);
                if (!File.Exists(filePath)) continue;

                var data = File.ReadAllText(filePath);
                //already updated
                if (data.Contains(entry.Value.First())) continue;

                var toAdd = string.Join(Environment.NewLine, entry.Value);
                const string endRoot = "</root>";
                data = data.Replace(endRoot, toAdd + Environment.NewLine + endRoot);
                File.WriteAllText(filePath, data);
            }
        }

        private static Dictionary<string, List<string>> GenerateTemplatedTranslations(string path, int startIndex, bool shouldPascalCase, string separator = ";")
        {
            var lines = File.ReadAllLines(path);
            SortTranslations(lines, separator);

            var template = File.ReadAllText(@"Translations\\template.txt");
            var languageMap = new Dictionary<int, string>();
            //language is the key
            var resultMap = new Dictionary<string, List<string>>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var words = line.Split(separator);
                if (words.All(string.IsNullOrEmpty))
                    continue;
                if (i == 0)
                {
                    for (int j = startIndex; j < words.Length; j++)
                    {
                        var language = words[j];
                        languageMap.Add(j, language);
                        resultMap.Add(language, new List<string>());
                    }
                    continue;
                }

                var baseWord = words[startIndex];
                if (shouldPascalCase)
                {
                    baseWord = baseWord.ToPascalCase();
                    baseWord = baseWord.Replace("(%)", string.Empty);
                }

                for (int j = startIndex; j < words.Length; j++)
                {
                    var word = words[j];
                    var replaced = template.Replace("{name}", baseWord).Replace("{value}", word);
                    resultMap[languageMap[j]].Add(replaced);
                }
            }

            return resultMap;
        }

        private static void SaveToFiles(Dictionary<string, List<string>> resultMap)
        {
            foreach (var replaced in resultMap)
            {
                var concat = string.Join(Environment.NewLine, replaced.Value);
                File.WriteAllText(@"Translations\\" + replaced.Key + ".txt", concat);
            }
        }

        private static void SortTranslations(string[] lines, string separator)
        {
            var result = lines.Skip(1).OrderBy(x => x.Split(separator).First()).ToList();
            result = new List<string> { lines.First() }.Concat(result).ToList();
            File.WriteAllLines(@"Translations\\Translations_Sorted.csv", result, Encoding.UTF8);
        }
    }
}