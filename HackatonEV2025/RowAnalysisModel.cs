namespace HackatonEV2025;

public class RowAnalysis
{
    public int RowId { get; set; }
    public string Comments { get; set; }
    public Dictionary<string, SuggestedScoreChange> SuggestedScores { get; set; }
}

public class SuggestedScoreChange
{
    public int Suggested { get; set; }
    public int Original { get; set; }
}

