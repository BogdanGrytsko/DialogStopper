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
        public void SortTranslations()
        {
            var path = @"Translations\\Translations.csv";
            var lines = File.ReadAllLines(path);
            var result = lines.Skip(1).OrderBy(x => x.First()).ToList();
            result = new List<string> { lines.First() }.Concat(result).ToList();
            File.WriteAllLines(@"Translations\\Translations_Sorted.csv", result, Encoding.UTF8);
        }
    }
}