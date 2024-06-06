using System.Net;
using System.Text;
using Azure;
using Azure.AI.OpenAI;
using summeringsmakker.Data;
using Newtonsoft.Json;
using summeringsmakker.Models;

namespace summeringsMakker.Services;

public class LegalReferenceValidator
{
    private readonly HttpClient httpClient = new HttpClient();
    private List<Message> messages = new List<Message>();
    private readonly SummeringsMakkerDbContext _context;
    private OpenAIClient client;

    public LegalReferenceValidator(SummeringsMakkerDbContext context)
    {
        _context = context;

        var GPT4V_KEY = File.ReadAllText("EnvVariables/gpt4v_key").Trim();
        httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10) // Set timeout to 5 minutes
        };
        httpClient.DefaultRequestHeaders.Add("api-key", GPT4V_KEY);
    }

    private const string GPT4V_ENDPOINT =
        "https://azureopenaitestsyl.openai.azure.com/openai/deployments/TeamHovedopgave/chat/completions?api-version=2024-02-15-preview";

    private const double TEMPERATURE = 0.1;
    private const double TOP_P = 0.95;
    private const int MAX_TOKENS = 4096;


    public async Task<List<LegalReference>> ValidateLegalReferences(List<LegalReference> legalReferences)
    {
        // Load legal document text from file
        string filePath = "legalDocumentShort.pdf";
        string legalDocument = File.Exists(filePath) ? TextExtractor.ExtractTextFromPdf(filePath) : string.Empty;

        var results = new List<(int, bool, bool)>();

        var formattedLegalReferencesList =
            string.Join("; ", legalReferences.Select((text, index) => $"id:{index}, text:{text}")); // todo fix this

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
                
                Svaret skal være en CSV-liste med 'id, IsActual, IsInEffect;' for hver henvisning, hvor IsInEffect og IsActual er angivet som 'true' eller 'false'."
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
            var response = await SendRequestToAi(JsonConvert.SerializeObject(payload), httpClient);
            Console.Write("123123");
            dynamic responseObj = JsonConvert.DeserializeObject(response);

            // Process each result from the AI response
            var legalReferenceStatus =
                ParseLegalReferenceStatus(responseObj["completions"][0]["data"]["text"].ToString());
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

        // Validate legal references
        if (legalReferences.Count > 0)
        {
            // live LLM check
            var truthTableResult = await ValidateLegalReferences(legalReferences);
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