using summeringsmakker.Data;
using Newtonsoft.Json;
using iText.Layout.Element;
using summeringsmakker.Services;

namespace summeringsMakker.Services;

public class Checker
{

    private readonly HttpClient httpClient = new HttpClient();
    private List<Message> messages = new List<Message>();
    private readonly SummeringsMakkerDbContext _context;

    public Checker(SummeringsMakkerDbContext context)
    {
        _context = context;

        var GPT4V_KEY = File.ReadAllText("EnvVariables/gpt4v_key").Trim();
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("api-key", GPT4V_KEY);
    }

    private const string GPT4V_ENDPOINT =
        "https://ftfaopenaiswedentest.openai.azure.com/openai/deployments/FTFA-gpt-4-vision-preview/chat/completions?api-version=2023-07-01-preview";

    private const double TEMPERATURE = 0.1;
    private const double TOP_P = 0.95;
    private const int MAX_TOKENS = 4096;

    
    public async Task<Dictionary<string, (bool, string)>> TruthTable(List<string> textList)
    {
            string filePath = "legalDocumentShort.pdf";
            //string filePath = "legalDocumentShortError.pdf";
            string legalDocument = string.Empty;

            if (File.Exists(filePath))
            {
                legalDocument = TextExtractor.ExtractTextFromPdf(filePath);
            }

            var results = new Dictionary<string, (bool, string)>();

            foreach (var text in textList)
            {
                var payload = new
                {
                    messages = new List<object>
                    {
                  
                        new { role = "system", content = "Du er en AI der modtager et stykke tekst, som du skal sammenligne med teksten i et juridisk dokument." },
                        new { role = "user", content = "Her er teksten fra det juridiske dokument:" },
                        new { role = "assistant", content = legalDocument },
                        new { role = "user", content = $"udfør de følgende opgaver i rækkefølge:" +
                        $"1: Sammenlign følgende tekst: {text} med det juridiske dokument {legalDocument}. Hvis teksten findes i dokumentet, svar da med 'true', ellers svar med 'false'." +
                        $"2: når du sammenligner følgende tekst: {text} med det juridiske dokument {legalDocument} skal du tjekke om ordet ophørt indgår i nogen af referencerne. hvis ja svar med 'ophørt' ellers svar med 'gældende'." }
                        
                    },
                    temperature = 0.1,
                    top_p = 0.95,
                    max_tokens = 4096,
                    stream = false
                };

                var response = await CaseProcessor.SendRequestToAI(JsonConvert.SerializeObject(payload), httpClient);
                dynamic responseObj = JsonConvert.DeserializeObject(response);

                string truthTableResponse = (string)responseObj.choices[0].message.content;
                bool isTrue = truthTableResponse.Contains("true");
                string status = truthTableResponse.Contains("ophørt") ? "ophørt" : "gældende";
                results[text] = (isTrue, status);

                // Add detailed logging
                Console.WriteLine($"Text: {text}, Found: {isTrue}, Status: {status}");
            }

            // Log the entire results dictionary
            Console.WriteLine("TruthTable Results: ");
            foreach (var kvp in results)
            {
                Console.WriteLine($"Key: {kvp.Key}, Value: Found - {kvp.Value.Item1}, Status - {kvp.Value.Item2}");
            }

            return results;
        }
}


