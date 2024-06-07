using Microsoft.AspNetCore.Mvc;
using summeringsMakker.Repository;
using summeringsMakker.Services;

namespace summeringsmakker.Controllers
{
    // private string LAW_API_ENDPOINT = "URL";
        
    [Controller]
    public class CaseSummaryUpdateController(
        ICaseSummaryRepository caseSummaryRepository,
        LegalReferenceValidator legalReferenceValidator)
        : Controller
    {
        
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
        
    }
}