namespace HackatonEV2025;

public class QuestionnaireResponse
{
    public DateTime Timestamp { get; set; }
    public string CompanyName { get; set; }
    public int Q1 { get; set; }
    public int Q2 { get; set; }
    public int Q3 { get; set; }
    public int Q4 { get; set; }
    public int Q5 { get; set; }
    public int Q6 { get; set; }
    public int Q7 { get; set; }
    public int Q8 { get; set; }
    public int Q9 { get; set; }
    public int Q10 { get; set; }
    public string Q11 { get; set; }

    public int RowId { get; set; }

    public string GetString(int rowId)
    {
        RowId = rowId;
        return $"rowId: {rowId}. Q1: {Q1}, Q2: {Q2}, Q3: {Q3}, Q4: {Q4}, Q5: {Q5}, Q6: {Q6}, Q7: {Q7}, Q8: {Q8}, Q9: {Q9}, Q10: {Q10}, Q11: {Q11},";
    }
}
