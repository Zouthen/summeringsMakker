namespace summeringsmakker.Models.DTO;

public class CaseSummaryDTO
{
    public int CaseSummaryId { get; set; }
    public string Summary { get; set; }
    public string MermaidCode { get; set; }
    public List<string> CaseSummaryWords { get; set; }
    //public List<string> CaseSummaryLegalReferences { get; set; }
    //public Dictionary<string, bool> CaseSummaryLegalReferences { get; set; }
    //public Dictionary<string, object> CaseSummaryLegalReferences { get; set; }
    //public Dictionary<string, (bool Found, string Status)> CaseSummaryLegalReferences { get; set; }
    public Dictionary<string, LegalReferenceStatus> CaseSummaryLegalReferences { get; set; }
    
}

public class LegalReferenceStatus
{
    public bool Found { get; set; }
    public string Status { get; set; }
}