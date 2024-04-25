using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using summeringsmakker.DTOs;
using System;
using summeringsmakker.Models;
using summeringsMakker.Repository;
using summeringsMakker.Services;


namespace summeringsmakker.Controllers
{
    [ApiController]
    public class CaseSummaryController : Controller
    {
        private readonly ILogger<CaseSummaryController> _logger;
        private readonly ICaseRepository _caseRepository;
        private readonly ICaseSummaryRepository _caseSummaryRepository;

        public CaseSummaryController(ILogger<CaseSummaryController> logger, ICaseRepository caseRepository, ICaseSummaryRepository caseSummaryRepository)
        {
            _logger = logger;
            _caseRepository = caseRepository;
            _caseSummaryRepository = caseSummaryRepository;
        }

        public CaseSummaryController(ILogger<CaseSummaryController> logger)
        {
            _logger = logger;
        }

        // public IActionResult Index()
        // {
        //     return View();
        // }
        
        [HttpPost("create-case-summaries")]
        public async Task<IActionResult> CreateCaseSummaries()
        {
            // specify fetch period to whole day
            DateTime startOfDay = DateTime.Today;
            DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            // Fetch all cases for the current day from the data warehouse
            var casesForPeriod = _caseRepository.GetCases(startOfDay, endOfDay);

            // Extract the IDs of the fetched cases
            var caseIdsForPeriod = casesForPeriod.Select(c => c.Id).ToList();

            // Get the IDs of the case summaries that already exist
            var existingCaseSummaryIds = _caseSummaryRepository.GetCaseSummariesIds(caseIdsForPeriod).ToHashSet();

            // Filter out the cases that already have summaries
            var casesWithoutSummaries = casesForPeriod
                .Where(c => !existingCaseSummaryIds.Contains(c.Id))
                .ToList();
            
            // create summaries
            var caseSummaries = new List<CaseSummary>();
        
            string filePath = "afgørelse.pdf"; // todo remove alter hardcoded path
            foreach (var caseWithoutSummary in casesWithoutSummaries)
            {
                var caseSummary = await CaseProcessor.ProcessFile(filePath);
                caseSummaries.Add(caseSummary);
            }

            // add to database
            _caseSummaryRepository.Add(caseSummaries);

            // give request response
            return Ok();
        }
        
        [HttpPost("create-case-summaries")]
        public IActionResult CreateCaseSummaries([FromBody] CaseSummariesRequest request)
        {
            // DateTime startDate = request.StartDate;
            // DateTime endDate = request.EndDate;

            // Logic to create case summaries from startDate to endDate

            // For demonstration purposes, let's just return a success message
            // return Ok($"Case summaries created from '{startDate}' to '{endDate}'");
            return Ok();
        }
    }
}
