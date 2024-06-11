namespace summeringsmakker.Models.DTO;

public class CaseSummaryDTO
{
    public int CaseSummaryId { get; set; }
    public string Summary { get; set; }
    public string MermaidCode { get; set; }
    public List<string> CaseSummaryWords { get; set; }
    public List<LegalReferenceStatus> CaseSummaryLegalReferences { get; set; }
}

public class LegalReferenceStatus
{
    public string Text { get; set; }
    public bool IsInEffect { get; set; }
    public bool IsActual { get; set; }
}