using System.Text.RegularExpressions;

namespace HackatonEV2025;

internal class RowAnalysisParser
{
    public static List<RowAnalysis> ParseResponse(string response)
    {
        var rows = new List<RowAnalysis>();
        var lines = response.Split('\n');

        // Regex for parsing suggested scores
        var scoreRegex = new Regex(@"Q(\d+): (\d+) \(instead of (\d+)\)");

        foreach (var line in lines)
        {
            // Skip the header or empty lines
            if (line.StartsWith("| rowId") || line.StartsWith("|-------") || string.IsNullOrWhiteSpace(line))
                continue;

            var columns = line.Split('|', StringSplitOptions.TrimEntries);

            if (columns.Length < 4)
                continue;

            var row = new RowAnalysis
            {
                RowId = int.Parse(columns[1]),
                Comments = columns[2],
                SuggestedScores = new Dictionary<string, SuggestedScoreChange>()
            };

            // Parse the suggested score column
            var scoreMatches = scoreRegex.Matches(columns[3]);
            foreach (Match match in scoreMatches)
            {
                var question = $"Q{match.Groups[1].Value}";
                var suggested = int.Parse(match.Groups[2].Value);
                var original = int.Parse(match.Groups[3].Value);

                row.SuggestedScores[question] = new SuggestedScoreChange
                {
                    Suggested = suggested,
                    Original = original
                };
            }

            rows.Add(row);
        }

        return rows;
    }
}
