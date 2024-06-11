using summeringsmakker.Data;

namespace summeringsMakker.Services;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using summeringsmakker.Models;

public class CaseProcessor
{
    private readonly HttpClient httpClient;
    private List<Message> _messages = new List<Message>();
    private readonly SummeringsMakkerDbContext _context;

    public CaseProcessor(SummeringsMakkerDbContext context)
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


    public async Task<CaseSummary> ProcessFile(Case caseItem)
    {

        var caseSummary = new CaseSummary
        {
            CaseId = caseItem.Id
        };
        string text = caseItem.Content;

        try
        {
            // Anonymize text
            string anonymizedText = await AnonymizeText(text);
            // Analyze text
            await AnalyzeText(caseSummary, anonymizedText);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing case {caseItem.Id}: {ex.Message}");
            throw; // Re-throw the exception to handle it further up the call stack if needed
        }

        return caseSummary;
    }

    private async Task<string> AnonymizeText(string text)
    {
        var Messages = new List<object>
        {
            new { role = "system", content = "You are an AI that redacts personal information from text." },
            new { role = "user", content = "Redact personal data from the provided text string using the following tokens: Replace names with 'person'. Replace dates with 'date'. Replace locations with 'location'. Replace organization names with 'organization'. Replace unique identifiers with 'identifier'. Replace any other personal information tokens with 'personal_info'. Replace descriptors for types of persons (e.g., 'plaintiff', 'defendant') with 'person_type'. Ensure that the redacted text maintains readability and preserves the essential legal context of the document." },
            new { role = "user", content = text }
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

        var response = await SendRequestToAI(JsonConvert.SerializeObject(payload), httpClient);
        dynamic responseObj = JsonConvert.DeserializeObject(response);
        string textAnonymized = (string)responseObj.choices[0].message.content;
        return textAnonymized;
    }




    private async Task AnalyzeText(CaseSummary viewModel, string text)
    {
        var messages = new List<object>
        {
            new { role = "system", content = "Du er en AI der scanner juridiske dokumenter og udtrækker de vigtigste dele og du svare på dansk" },
            new { role = "user", content = "brug den juridiske metode når du analysere dokumenter" },
        };

        await SendTextForSummary(viewModel, text);
        await AnalyzeWordFrequency(viewModel, text);
        await GenerateMermaidDiagram(viewModel, text);
        await FindLegalReferences(viewModel, text);
    }

    private async Task SendTextForSummary(CaseSummary viewModel, string text)
    {
        var Messages = new List<object>
    {
        new { role = "system", content = "Lav et resume af den givne tekst på dansk" },
        new { role = "user", content = text }
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

        var response = await SendRequestToAI(JsonConvert.SerializeObject(payload), httpClient);
        Console.WriteLine("Summary: " + response);
        dynamic responseObj = JsonConvert.DeserializeObject(response);
        string tempSummary = (string)responseObj.choices[0].message.content;
        viewModel.Summary = tempSummary.Replace("Resume:", "").Trim();
    }


    private async Task AnalyzeWordFrequency(CaseSummary viewModel, string text)
    {
        var Messages = new List<object>
    {
        new { role = "system", content = "Identificer de 10 vigtigste ord i teksten og arranger dem efter deres hyppighed på følgende måde: ord - hyppighed." },
        new { role = "user", content = text }
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

        var response = await SendRequestToAI(JsonConvert.SerializeObject(payload), httpClient);
        Console.WriteLine("Word Frequencies: " + response);
        dynamic responseObj = JsonConvert.DeserializeObject(response);
        string wordFrequencies = responseObj.choices[0].message.content;

        string[] lines = wordFrequencies.Split('\n');

        foreach (string line in lines)
        {
            var parts = line.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                string wordsPart = parts[0].Replace("-", "").Trim();
                wordsPart = wordsPart.Substring(wordsPart.IndexOf('.') + 1).Trim(); // Remove the numbering.
                int frequencyPart = int.Parse(parts[1].Trim());

                var word = _context.Words.FirstOrDefault(w => w.Text == wordsPart) ?? new Word { Text = wordsPart };
                var caseSummaryWord = new CaseSummaryWord
                { Word = word, CaseSummary = viewModel, Frequency = frequencyPart };

                viewModel.CaseSummaryWords.Add(caseSummaryWord);
                word.CaseSummaryWords.Add(caseSummaryWord);
            }
        }
    }

    private async Task GenerateMermaidDiagram(CaseSummary viewModel, string text)
    {
        var Messages = new List<object>
        {
            new { role = "system", content = "Generate a sequence Mermaid diagram from the flow in the following text. Do not add reminders or other unnecessary text" },
            new { role = "user", content = text }
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

        var jsonContent = JsonConvert.SerializeObject(payload);
        var response = await SendRequestToAI(jsonContent, httpClient);
        Console.WriteLine("Mermaid Diagram: " + response);
        dynamic responseObj = JsonConvert.DeserializeObject(response);
        string mermaidTemp = (string)responseObj.choices[0].message.content;
        viewModel.MermaidCode = mermaidTemp.Replace("```mermaid", "").Replace("```", "").Replace("(EF)", "EF").Trim();
    }

    private async Task FindLegalReferences(CaseSummary viewModel, string text)
    {
        var Messages = new List<object>
        {
            new { role = "system", content = "Identify and list all legal references in the text provided and return them exactly as you found them with no deviation." },
            new { role = "user", content = text }
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


        var response = await SendRequestToAI(JsonConvert.SerializeObject(payload), httpClient);
        Console.WriteLine("Legal References Found: " + response);

        dynamic responseObj = JsonConvert.DeserializeObject(response);
        string legalReferences = (string)responseObj.choices[0].message.content;

        if (!string.IsNullOrEmpty(legalReferences))
        {
            var references = legalReferences.Split('\n').Select(s => s.Trim()).ToList();
            foreach (var reference in references)
            {
                var legalReference = new LegalReference { Text = reference };
                var caseSummaryLegalReference = new CaseSummaryLegalReference
                    { LegalReference = legalReference, CaseSummary = viewModel };

                //the following line is for the DB, creating another entity (join table reference) this is done using pointers
                legalReference.CaseSummaryLegalReferences.Add(caseSummaryLegalReference);
                viewModel.CaseSummaryLegalReferences.Add(caseSummaryLegalReference);
            }
        }
    }

    public static async Task<string> SendRequestToAI(string jsonContent, HttpClient httpClient)
    {
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(GPT4V_ENDPOINT, content);

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
}

class Message
{
    public string role { get; set; } = string.Empty;
    public string content { get; set; } = string.Empty;
}