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
            GenerateTemplatedTranslations(inputPath, startIndex, shouldPascalCase);
        }

        private static void GenerateTemplatedTranslations(string path, int startIndex, bool shouldPascalCase, string separator = ";")
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