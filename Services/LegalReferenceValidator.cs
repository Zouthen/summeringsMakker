using System.Net;
using System.Text;
using System.Text.Json;
using Azure.AI.OpenAI;
using summeringsmakker.Data;
using Newtonsoft.Json;
using summeringsmakker.Models;
using summeringsmakker.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace summeringsMakker.Services;

public class LegalReferenceValidator
{
    private readonly HttpClient httpClient = new HttpClient();
    private List<Message> messages = new List<Message>();
    private readonly SummeringsMakkerDbContext _context;
    private OpenAIClient client;
    private readonly ICaseRepository _caseRepository;

    public LegalReferenceValidator(SummeringsMakkerDbContext context,
        ICaseRepository caseRepository) // Modify this line
    {
        _context = context;
        _caseRepository = caseRepository;

        var GPT4V_KEY = Environment.GetEnvironmentVariable("AI_KEY");
        httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10) // timeout
        };
        httpClient.DefaultRequestHeaders.Add("api-key", GPT4V_KEY);
    }

    private const string GPT4V_ENDPOINT =
        "https://azureopenaitestsyl.openai.azure.com/openai/deployments/TeamHovedopgave/chat/completions?api-version=2024-02-15-preview";

    private const double TEMPERATURE = 0.1;
    private const double TOP_P = 0.95;
    private const int MAX_TOKENS = 4096;


    public async Task<List<LegalReference>> ValidateLegalReferences(List<LegalReference> legalReferences,
        int caseSummary)
    {
        //  read from db
        var caseContext = _caseRepository.GetById(caseSummary);


        // Load legal document text from file
        string filePath = "legalDocumentShort.pdf";
        string legalDocument = File.Exists(filePath) ? TextExtractor.ExtractTextFromPdf(filePath) : string.Empty;

        var results = new List<(int, bool, bool)>();

        var formattedLegalReferencesList =
            string.Join("; ", legalReferences.Select((lr, index) => $"id:{index}, text:{lr.Text}")); // todo fix this

        // Construct payload for the AI request
        var payload = new
        {
            messages = new List<object>
            {
                new
                {
                    role = "system",
                    content =
                        "Du er et nøjagtig juridisk validerings program der modtager en liste af juridiske henvisninger med id for hver, hvor du skal sammenligne disse juridiske henvisninger med et juridisk dokument og returnere en csv fil med svar på spørgsmål besvaret ved at sammenligne det juridisk dokument med hver juridisk henvisning."
                },
                new { role = "user", content = $"juridiske dokument:" + legalDocument },
                new
                {
                    role = "user",
                    content =
                        $@"Listen af juridiske henvisninger: [{formattedLegalReferencesList}]
                For hver henvisning udfør følgende:
                - IsActual: Valider om den givne henvisning optræder i det juridiske dokument.
                - IsInEffect: Valider om henvisningen er ophørt. Dette gøres primært ved at tjekke om ordet 'ophørt' indgår.
                
                Svaret skal være en CSV-liste med 'id, IsActual, IsInEffect;' for hver henvisning, hvor IsInEffect og IsActual er angivet som 'true' eller 'false'.

                Dette er den konkrete juridiske sag der behandles: {caseContext}"
                }
            },
            temperature = 0.1,
            top_p = 0.95,
            max_tokens = 4096,
            stream = false
        };

        // Send request to AI and parse response
        try
        {
            string response = "{\"choices\":[{\"content_filter_results\":{\"hate\":{\"filtered\":false,\"severity\":\"safe\"},\"self_harm\":{\"filtered\":false,\"severity\":\"safe\"},\"sexual\":{\"filtered\":false,\"severity\":\"safe\"},\"violence\":{\"filtered\":false,\"severity\":\"safe\"}},\"finish_reason\":\"stop\",\"index\":0,\"logprobs\":null,\"message\":{\"content\":\"id,IsActual,IsInEffect\\n0,true,true\\n1,true,true\\n2,true,true\\n3,true,true\\n4,false,false\\n5,false,false\\n6,false,false\",\"role\":\"assistant\"}}],\"created\":1717751702,\"id\":\"chatcmpl-9XPyYDbafHykMNSVZZ1F9vlUYBDlC\",\"model\":\"gpt-4-turbo-2024-04-09\",\"object\":\"chat.completion\",\"prompt_filter_results\":[{\"prompt_index\":0,\"content_filter_results\":{\"hate\":{\"filtered\":false,\"severity\":\"safe\"},\"self_harm\":{\"filtered\":false,\"severity\":\"safe\"},\"sexual\":{\"filtered\":false,\"severity\":\"safe\"},\"violence\":{\"filtered\":false,\"severity\":\"safe\"}}}],\"system_fingerprint\":\"fp_f5ce1e47b6\",\"usage\":{\"completion_tokens\":36,\"prompt_tokens\":576,\"total_tokens\":612}}\n";
            // string response = await SendRequestToAi(JsonConvert.SerializeObject(payload), httpClient);
            JsonDocument json = JsonDocument.Parse(response);
            string? content = json.RootElement
                .GetProperty("choices")
                .EnumerateArray().First()
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            // Process each result from the AI response
            var legalReferenceStatus = ParseLegalReferenceStatus(content);
            for (int i = 0; i < legalReferences.Count; i++)
            {
                var legalReference = legalReferences[i];
                if (legalReferenceStatus.TryGetValue(i, out (bool IsActual, bool IsInEffect) status))
                {
                    legalReference.IsActual = status.IsActual;
                    legalReference.IsInEffect = status.IsInEffect;
                }
                else
                {
                    legalReference.IsActual = false;
                    legalReference.IsInEffect = false;
                }
            }


            return legalReferences;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task<string> SendRequestToAi(string jsonContent, HttpClient httpClient)
    {
        if (jsonContent == null)
        {
            Console.WriteLine("jsonContent is null");
            return null;
        }

        try
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(GPT4V_ENDPOINT, content);

            // Get the HTTP status code
            HttpStatusCode statusCode = response.StatusCode;
            Console.WriteLine($"HTTP Status Code: {statusCode}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Failed to send payload. Status code: {response.StatusCode}.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<CaseSummary> ValidateCaseSummaryLegalReferences(CaseSummary caseSummary)
    {
        var legalReferences = caseSummary.GetLegalReferences();

        var caseId = caseSummary.CaseId;

        // Validate legal references
        if (legalReferences.Count > 0)
        {
            // live LLM check
            var truthTableResult = await ValidateLegalReferences(legalReferences, caseId);
        }

        caseSummary.LastChecked = DateTime.Now;

        return caseSummary;
    }

    // helper methods
    private Dictionary<int, (bool IsActual, bool IsInEffect)> ParseLegalReferenceStatus(string csvResponse)
    {
        var legalReferenceStatus = new Dictionary<int, (bool IsActual, bool IsInEffect)>();

        var csvLines = csvResponse.Split('\n');
        foreach (var line in csvLines)
        {
            if (line.ToLower().Contains("id")) continue;
            
            var values = line.Trim(';').Split(',');
            if (values.Length != 3) continue;
            legalReferenceStatus.Add(
                int.Parse(values[0]),
                (bool.Parse(values[1]), bool.Parse(values[2]))
            );
        }

        return legalReferenceStatus;
    }
}