using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using summeringsmakker.DTOs;
using System;
using summeringsmakker.Models;
using summeringsMakker.Repository;
using summeringsmakker.Services;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using summeringsmakker.Models.DTO;

// using summeringsMakker.Services;


namespace summeringsmakker.Controllers
{
    //[ApiController]
    [Controller]
    public class CaseSummaryController : Controller
    {
        private readonly ILogger<CaseSummaryController> _logger;
        private readonly ICaseRepository _caseRepository;
        private readonly ICaseSummaryRepository _caseSummaryRepository;

        public CaseSummaryController(ILogger<CaseSummaryController> logger, ICaseRepository caseRepository,
            ICaseSummaryRepository caseSummaryRepository)
        {
            _logger = logger;
            _caseRepository = caseRepository;
            _caseSummaryRepository = caseSummaryRepository;
        }


        // public IActionResult Index()
        // {
        //     return View();
        // }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoadCases(CaseSummary caseSummary, CancellationToken cancellationToken)
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var caseList = _caseSummaryRepository.GetCaseSummaries();

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
                {
                    var isDescending = sortColumnDir.Equals("desc", StringComparison.OrdinalIgnoreCase);

                    caseList = sortColumn switch
                    {
                        "Id" => isDescending
                            ? caseList.OrderByDescending(t => t.CaseSummaryId).ToList()
                            : caseList.OrderBy(t => t.CaseSummaryId).ToList(),
                        _ => caseList
                    };
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var normalizedSearchValue = SearchUtility.NormalizeString(searchValue);

                    caseList = caseList.Where(caseSummary =>
                        SearchUtility.NormalizeString(caseSummary.CaseSummaryId.ToString())
                            .Contains(normalizedSearchValue) ||
                        caseSummary.GetWords().Any(word => SearchUtility.NormalizeString(word.Text).Contains(normalizedSearchValue))
                    ).ToList();
                }

                // Using DTO to avoid circular reference
                var caseSummaryDtoList = caseList.Select(caseSummary => new CaseSummaryDTO
                {
                    CaseSummaryId = caseSummary.CaseSummaryId,
                    Summary = caseSummary.Summary,
                    MermaidCode = caseSummary.MermaidCode,
                    CaseSummaryWords = caseSummary.GetWords().Select(word => word.Text).ToList(),
                    CaseSummaryLegalReferences = caseSummary.GetLegalReferences().Select(legalReference => legalReference.Text).ToList()
                }).ToList();

                var data = caseSummaryDtoList.Skip(skip).Take(pageSize).ToList();
                recordsTotal = caseSummaryDtoList.Count();

                return Json(new
                    { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
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
                // var caseSummary = await CaseProcessor.ProcessFile(filePath);
                // caseSummaries.Add(caseSummary);
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

        // get case summaries by word [liste af words]
    }
}