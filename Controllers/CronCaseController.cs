using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using summeringsmakker.Repository;
using summeringsmakker.Models;
using summeringsMakker.Repository;
using summeringsMakker.Services;

namespace summeringsmakker.Controllers
{
    [ApiController]
    [Route("cron")]
    public class CronCaseController : Controller
    {
        private readonly ICaseRepository _caseRepository;
        private readonly ICaseSummaryRepository _caseSummaryRepository;
        private readonly CaseProcessor _caseProcessor;

        public CronCaseController(
            ICaseRepository caseRepository,
            ICaseSummaryRepository caseSummaryRepository,
            CaseProcessor caseProcessor)
        {
            _caseRepository = caseRepository;
            _caseSummaryRepository = caseSummaryRepository;
            _caseProcessor = caseProcessor;
        }

        [HttpPost("create-case-summaries")]
        public async Task<IActionResult> CreateCaseSummaries()
        {
            // specify fetch period to whole day
            DateTime startOfDay = DateTime.Today;
            DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            // Fetch all cases for the current day from the data warehouse
            var casesForPeriod = _caseRepository.GetAll(startOfDay, endOfDay);

            // Extract the IDs of the fetched cases
            var caseIdsForPeriod = casesForPeriod.Select(c => c.Id).ToList();

            // Get the IDs of the case summaries that already exist
            var existingCaseIds = _caseSummaryRepository.GetCaseIds(caseIdsForPeriod);

            // Filter out the cases that already have summaries
            var casesWithoutSummaries = casesForPeriod
                .Where(c => !existingCaseIds.Contains(c.Id))
                .ToList();

            // create summaries
            var caseSummaries = new List<CaseSummary>();

            foreach (var caseWithoutSummary in casesWithoutSummaries)
            {
                var caseSummary = await _caseProcessor.ProcessFile(caseWithoutSummary);
                caseSummaries.Add(caseSummary);
            }

            // add to database
            _caseSummaryRepository.Add(caseSummaries);

            var message = casesWithoutSummaries.Count > 0
                ? $"{casesWithoutSummaries.Count} case(s) were added"
                : "Already up to date";

            // Pass the message to the view
            TempData["Notification"] = message;

            return RedirectToAction("Index", "CaseSummary");
        }
    }
}
