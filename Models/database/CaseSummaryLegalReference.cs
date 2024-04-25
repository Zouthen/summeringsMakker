namespace summeringsmakker.Models;

public class CaseSummaryLegalReference
{
    public int CaseSummaryId { get; set; }
    public CaseSummary CaseSummary { get; set; }
    public int LegalReferenceId { get; set; }
    public LegalReference LegalReference { get; set; }
}