using summeringsmakker.Data;
using Newtonsoft.Json;
using iText.Layout.Element;
using summeringsmakker.Services;
using Azure;
using MySqlX.XDevAPI;

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

    private const string GPT4V_ENDPOINT = "https://azureopenaitestsyl.openai.azure.com/openai/deployments/TeamHovedopgave/chat/completions?api-version=2024-02-15-preview";

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
                var Messages = new List<object>
                {
                
                    new { role = "system", content = "Du er en AI der modtager et stykke tekst, som du skal sammenligne med teksten i et juridisk dokument." },
                    new { role = "user", content = "Her er teksten fra det juridiske dokument:" },
                    new { role = "assistant", content = legalDocument },
                    new { role = "user", content = $"udfør de følgende opgaver i rækkefølge:" +
                    $"1: Sammenlign følgende tekst: {text} med det juridiske dokument {legalDocument}. Hvis teksten findes i dokumentet, svar da med 'true', ellers svar med 'false'." +
                    $"2: når du sammenligner følgende tekst: {text} med det juridiske dokument {legalDocument} skal du tjekke om ordet ophørt indgår i nogen af referencerne. hvis ja svar med 'ophørt' ellers svar med 'gældende'." }
                    
                };

                var payload = new
                {
                    messages = Messages,
                    temperature = TEMPERATURE,
                    max_tokens = MAX_TOKENS,
                    top_p = (float)TOP_P,
                    frequency_penalty = 0,
                    presence_penalty = 0
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


