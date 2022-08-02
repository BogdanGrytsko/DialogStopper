using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FuzzySharp;
using Xunit;

namespace PlayerMap.BasketballReference
{
    public class FuzzyMatchTests
    {
        [Fact]
        public void TestNames()
        {
            var pairs = new List<(string, string)>()
            {
                ("Lebron James", "Lebron  James"),
                ("Bill smith jr.", "bill smith jr"),
                ("T.j. Ford", "tj ford")
            };
            var results = pairs.Select(x => Fuzz.Ratio(x.Item1.Replace(".", string.Empty).ToUpper(),
                x.Item2.Replace(".", string.Empty).ToUpper())).ToList();
            foreach (var result in results)
            {
                result.Should().BeGreaterOrEqualTo(95);
            }
        }
    }
}