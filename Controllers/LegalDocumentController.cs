using System.Text;
using Microsoft.AspNetCore.Mvc;
using summeringsMakker.Repository;
using summeringsMakker.Services;
using iTextSharp.text.pdf.parser;
using Path = System.IO.Path;
using PdfReader = iTextSharp.text.pdf.PdfReader;
using System.Text.RegularExpressions;
using System.IO;



namespace summeringsmakker.Controllers;

[Controller]
public class LegalDocumentController(
    ICaseSummaryRepository caseSummaryRepository,
    LegalReferenceValidator legalReferenceValidator)
    : Controller
{
    private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "LegalDocuments");
    private readonly string _legalDocumentUrl = "https://www.retsinformation.dk/api/pdf/238690";


    // POST: CaseSummaryUpdateController/UpdateLastChecked
    [HttpPost]
    public async Task<IActionResult> UpdateLastChecked(DateTime? date)
    {
        var dateToCheck = date ?? DateTime.Now;

        var caseSummaries = caseSummaryRepository.GetCaseSummaries()
            .Where(cs => cs.LastChecked < dateToCheck).ToList();

        // Validate legal references
        caseSummaries.ForEach(cs =>
            _ = legalReferenceValidator.ValidateCaseSummaryLegalReferences(cs));

        // Update database
        caseSummaryRepository.Update(caseSummaries);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DownloadAndProcessLegalDocument()
    {
        var filePath = Path.Combine(_filePath, "legalDoc.pdf");
        var textFilePath = Path.Combine(_filePath, "legalDoc.txt");

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(_legalDocumentUrl);

        if (response.IsSuccessStatusCode)
        {
            var bytes = await response.Content.ReadAsByteArrayAsync();
            await System.IO.File.WriteAllBytesAsync(filePath, bytes);

            // Convert PDF to text
            var reader = new PdfReader(filePath);
            var textBuilder = new StringBuilder();

            for (var page = 1; page <= reader.NumberOfPages; page++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                var currentText = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
                textBuilder.Append(currentText);
            }

            reader.Close();

            // Write the extracted text to legalDoc.txt
            await System.IO.File.WriteAllTextAsync(textFilePath, textBuilder.ToString());
            
            return Ok("File saved successfully.");
        }
        else
        {
            return BadRequest("Failed to download file.");
        }
    }
}