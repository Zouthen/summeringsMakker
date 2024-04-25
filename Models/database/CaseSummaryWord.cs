namespace summeringsmakker.Models;

public class CaseSummaryWord
{
    public int CaseSummaryId { get; set; }
    public CaseSummary CaseSummary { get; set; }
    public int WordId { get; set; }
    public Word Word { get; set; }
    public int Frequency { get; set; }
}