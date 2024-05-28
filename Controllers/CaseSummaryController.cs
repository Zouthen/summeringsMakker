using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using summeringsmakker.DTOs;
using System;
using summeringsmakker.Models;
using summeringsMakker.Repository;
using summeringsmakker.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using summeringsmakker.Models.DTO;
using summeringsMakker.Services;

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
        private readonly Checker _checker;
        private readonly CaseProcessor _caseProcessor;

        public CaseSummaryController(ILogger<CaseSummaryController> logger, ICaseRepository caseRepository,
            ICaseSummaryRepository caseSummaryRepository, Checker checker, CaseProcessor caseProcessor)
        {
            _logger = logger;
            _caseRepository = caseRepository;
            _caseSummaryRepository = caseSummaryRepository;
            _checker = checker;
            _caseProcessor = caseProcessor;
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

        var legalReferences = caseList
            .SelectMany(caseSummary => caseSummary.GetLegalReferences().Select(legalReference => legalReference.Text))
            .Distinct()
            .ToList();

        var truthTableResult = await _checker.TruthTable(legalReferences);

        var caseSummaryDtoList = caseList.Select(caseSummary => new CaseSummaryDTO    
        {
            CaseSummaryId = caseSummary.CaseSummaryId,
            Summary = caseSummary.Summary.Split('.').FirstOrDefault() + "." ?? string.Empty,
            MermaidCode = caseSummary.MermaidCode,
            CaseSummaryWords = caseSummary.GetWords().Select(word => word.Text).ToList(),
            CaseSummaryLegalReferences = caseSummary.GetLegalReferences()
            .Select(legalReference => legalReference.Text)
            .ToDictionary(
                text => text,
                text => truthTableResult.TryGetValue(text, out var result) 
                    ? new LegalReferenceStatus { Found = result.Item1, Status = result.Item2 }
                    : new LegalReferenceStatus { Found = false, Status = "not found" }
            )
        }).ToList();

        _logger.LogInformation("CaseSummaryDTO List: {@caseSummaryDtoList}", caseSummaryDtoList);

        var data = caseSummaryDtoList.Skip(skip).Take(pageSize).ToList();
        recordsTotal = caseSummaryDtoList.Count();

        return Json(new
        { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error loading cases");
        return Json(new { error = ex.Message });
    }
}


        // GET: CaseSummaryController/Details/id
        public async Task<IActionResult> Details(int id)
        {
            var caseSummary = _caseSummaryRepository.GetById(id);   

            if (caseSummary == null)
            {
                return NotFound();
            }

            var legalReferences = caseSummary.CaseSummaryLegalReferences
            .Select(cslr => cslr.LegalReference.Text)
            .Distinct()
            .ToList();

            // live check
            var truthTableResult = await _checker.TruthTable(legalReferences);

            var caseSummaryDto = new CaseSummaryDTO    
            {
                CaseSummaryId = caseSummary.CaseSummaryId,
                Summary = caseSummary.Summary,
                MermaidCode = caseSummary.MermaidCode,
                CaseSummaryWords = caseSummary.GetWords().Select(word => word.Text).ToList(),
                CaseSummaryLegalReferences = caseSummary.GetLegalReferences()
                .Select(legalReference => legalReference.Text)
                .ToDictionary(
                    text => text,
                    text => truthTableResult.TryGetValue(text, out var result) 
                        ? new LegalReferenceStatus { Found = result.Item1, Status = result.Item2 }
                        : new LegalReferenceStatus { Found = false, Status = "not found" }
                ),
            };

            return View(caseSummaryDto);
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
                var caseSummary = await _caseProcessor.ProcessFile(caseWithoutSummary);
                caseSummaries.Add(caseSummary);
            }

            // add to database
            _caseSummaryRepository.Add(caseSummaries);

            // give request response
            return Ok();
        }

    }
}