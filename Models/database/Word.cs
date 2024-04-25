namespace summeringsmakker.Models;

public class Word
{
    public int WordId { get; set; }
    public string Text { get; set; }
    public List<CaseSummaryWord> CaseSummaryWords { get; set; }
    
    public Word()
    {
        CaseSummaryWords = new List<CaseSummaryWord>();
    }
}