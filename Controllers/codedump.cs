/*
namespace summeringsmakker.Controllers
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;
    using iText.Kernel.Pdf;
    using iText.Kernel.Pdf.Canvas.Parser;
    using iText.Kernel.Pdf.Canvas.Parser.Listener;
    using System.Text.RegularExpressions;
    
    internal class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static List<Message> messages = new List<Message>();

        static Program()
        {
            var GPT4V_KEY = Environment.GetEnvironmentVariable("GPT4V_KEY");
            httpClient.DefaultRequestHeaders.Add("api-key", GPT4V_KEY);
        }

        private const string GPT4V_ENDPOINT = "https://ftfaopenaiswedentest.openai.azure.com/openai/deployments/FTFA-gpt-4-vision-preview/chat/completions?api-version=2023-07-01-preview";
        private const double TEMPERATURE = 0.1;
        private const double TOP_P = 0.95;
        private const int MAX_TOKENS = 4096;

        static async Task Main(string[] args)
        {
            messages.Add(new Message { role = "system", content = "Du er en AI der scanner juridiske dokumenter og udtrækker de vigtigtste dele og du svare på dansk" });
            messages.Add(new Message { role = "user", content = "brug den juridiske metode når du analysere dokumenter" });

            Console.WriteLine("Enter the path to the PDF file:");
            string filePath = Console.ReadLine();

            if (File.Exists(filePath))
            {
                string text = ExtractTextFromPdf(filePath);
                messages.Add(new Message { role = "system", content = "Extracted text from PDF" });
                messages.Add(new Message { role = "user", content = text });
                await AnalyzeText(text);
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }

            Console.WriteLine("Processing completed.");
        }

        private static string ExtractTextFromPdf(string path)
        {
            using (PdfReader reader = new PdfReader(path))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    PdfPage page = pdfDoc.GetPage(i);
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                    text.AppendLine(pageText);
                }

                return text.ToString();
            }
        }

        private static async Task AnalyzeText(string text)
        {
            await SendTextForSummary(text);
            AnalyzeWordFrequency(text);
            await GenerateMermaidDiagram(text);
            FindLegalReferences(text);
        }

        private static async Task SendTextForSummary(string text)
        {
            var payload = new
            {
                messages = new List<object>
            {
                new { role = "system", content = "Please summarize the following text." },
                new { role = "user", content = text }
            },
                temperature = TEMPERATURE,
                top_p = TOP_P,
                max_tokens = 150,
                stream = false
            };

            var response = await SendRequestToOpenAI(JsonConvert.SerializeObject(payload));
            Console.WriteLine("Summary: " + response);
        }

        private static void AnalyzeWordFrequency(string text)
        {
            var payload = new
            {
                messages = new List<object>
            {
                new { role = "system", content = "Identify the 10 most important words and list the frequency of each of those words in the text." },
                new { role = "user", content = text }
            },
                temperature = TEMPERATURE,
                top_p = TOP_P,
                max_tokens = 100,
                stream = false
            };

            Console.WriteLine("Word Frequencies: ");

        }

        private static async Task GenerateMermaidDiagram(string text)
        {
            var payload = new
            {
                messages = new List<object>
            {
                new { role = "system", content = "Generate a Mermaid diagram from the flow in the following text." },
                new { role = "user", content = text }
            },
                temperature = TEMPERATURE,
                top_p = TOP_P,
                max_tokens = 100,
                stream = false
            };

            var response = await SendRequestToOpenAI(JsonConvert.SerializeObject(payload));
            Console.WriteLine("Mermaid Diagram: " + response);
        }

        private static void FindLegalReferences(string text)
        {
            var matches = Regex.Matches(text, @"\b§\s*\d+\b");
            Console.WriteLine("Legal References Found:");
            foreach (Match match in matches)
            {
                Console.WriteLine(match.Value);
            }
        }

        private static async Task<string> SendRequestToOpenAI(string jsonContent)
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
    

}
*/
