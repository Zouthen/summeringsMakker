namespace summeringsmakker.Models;


public class CaseSummary
{
    public int Id { get; set; }
    public string Summary { get; set; }
    public string mermaidCode { get; set; }
    public Array<String> LegalReferences { get; set; }
    public Array<Word> words { get; set; }
}
