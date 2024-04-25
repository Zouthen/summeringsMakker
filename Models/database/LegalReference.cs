namespace summeringsmakker.Models;

public class LegalReference
{
    public int LegalReferenceId { get; set; }
    public string Text { get; set; }
    public List<CaseSummaryLegalReference> CaseSummaryLegalReferences { get; set; }
    
    public LegalReference()
    {
        CaseSummaryLegalReferences = new List<CaseSummaryLegalReference>();
    }
}