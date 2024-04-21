namespace summeringsmakker.Models
{
    public class TextProcessed
    {
        public string Summary { get; set; }
        public string WordFrequencies { get; set; }
        public string MermaidDiagram { get; set; }
        public List<string> LegalReferences { get; set; } = new List<string>();
    }

}
