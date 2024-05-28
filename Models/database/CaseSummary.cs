namespace summeringsmakker.Models;

public class CaseSummary
{
    public int CaseSummaryId { get; set; }
    public int CaseId { get; set; }
    public string Summary { get; set; }
    public string MermaidCode { get; set; }
    public List<CaseSummaryWord> CaseSummaryWords { get; set; }
    public List<CaseSummaryLegalReference> CaseSummaryLegalReferences { get; set; }
    
    public CaseSummary()
    {
        CaseSummaryWords = new List<CaseSummaryWord>();
        CaseSummaryLegalReferences = new List<CaseSummaryLegalReference>();
    }
    
    public List<Word> GetWords()
    {
        return CaseSummaryWords.Select(csw => csw.Word).ToList();
    }

    public List<LegalReference> GetLegalReferences()
    {
        return CaseSummaryLegalReferences.Select(cslr => cslr.LegalReference).ToList();
    }
}