namespace summeringsmakker.Models;

public class CaseSummary
{
    public int Id { get; set; }
    public string Summary { get; set; }
    public string MermaidCode { get; set; }
    public List<string> LegalReferences { get; set; }
    public List<Word> Words { get; set; }
}


