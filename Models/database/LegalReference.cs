namespace summeringsmakker.Models;

public class LegalReference
{
    public int LegalReferenceId { get; set; }
    public string Text { get; set; }
    public bool IsActual { get; set; }
    public bool IsInEffect { get; set; }
    public List<CaseSummaryLegalReference> CaseSummaryLegalReferences { get; set; }

    public LegalReference()
    {
        CaseSummaryLegalReferences = new List<CaseSummaryLegalReference>();
        IsActual = false;
        IsInEffect = false;
    }
}