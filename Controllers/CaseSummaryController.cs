using Microsoft.AspNetCore.Mvc;
using summeringsmakker.Models;
using summeringsmakker.Services;
using summeringsmakker.Models.DTO;
using summeringsmakker.Repository;
using summeringsMakker.Repository;
using summeringsMakker.Services;

namespace summeringsmakker.Controllers
{
    [Controller]
    public class CaseSummaryController : Controller
    {
        private readonly ILogger<CaseSummaryController> _logger;
        private readonly ICaseRepository _caseRepository;
        private readonly ICaseSummaryRepository _caseSummaryRepository;
        private readonly LegalReferenceValidator _legalReferenceValidator;
        private readonly CaseProcessor _caseProcessor;

        public CaseSummaryController(ILogger<CaseSummaryController> logger, ICaseRepository caseRepository,
            ICaseSummaryRepository caseSummaryRepository, LegalReferenceValidator legalReferenceValidator, CaseProcessor caseProcessor)
        {
            _logger = logger;
            _caseRepository = caseRepository;
            _caseSummaryRepository = caseSummaryRepository;
            _legalReferenceValidator = legalReferenceValidator;
            _caseProcessor = caseProcessor;
        }

        // GET: CaseSummaryController/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: CaseSummaryController/Index
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

                //var truthTableResult = await _legalReferenceValidator.ValidateLegalReferences(legalReferences);
                
                caseList.ForEach(cs => cs.LastChecked = DateTime.Now);
                

                var caseSummaryDto = new CaseSummaryDTO
                {
                    CaseSummaryId = caseSummary.CaseSummaryId,
                    Summary = caseSummary.Summary,
                    MermaidCode = caseSummary.MermaidCode,
                    CaseSummaryWords = caseSummary.GetWords().Select(word => word.Text).ToList(),
                    /*
                    CaseSummaryLegalReferences = caseSummary.GetLegalReferences()
                        .Select(legalReference =>
                        {
                            if (truthTableResult != null && truthTableResult.TryGetValue(legalReference.Text, out var result))
                            {
                                return new LegalReferenceStatus { Text = legalReference.Text, IsInEffect = result.Item1, IsActual = result.Item2 };
                            }
                            else
                            {
                                return new LegalReferenceStatus { Text = legalReference.Text, IsInEffect = false, IsActual = "not found" };
                            }
                        }).ToList()
                    */
                    CaseSummaryLegalReferences = caseSummary.GetLegalReferences()
                    .Select(lr => new LegalReferenceStatus { Text = lr.Text, IsActual = lr.IsActual, IsInEffect = lr.IsInEffect })
                    .ToList()
                };

                var caseSummaryDtoList = caseList.Select(caseSummary => new CaseSummaryDTO
                {
                    CaseSummaryId = caseSummary.CaseSummaryId,
                    Summary = caseSummary.Summary.Split('.').FirstOrDefault() + "." ?? string.Empty,
                    MermaidCode = caseSummary.MermaidCode,
                    CaseSummaryWords = caseSummary.GetWords().Select(word => word.Text).ToList(),
                    CaseSummaryLegalReferences = caseSummary.GetLegalReferences()
                    .Select(lr => new LegalReferenceStatus { Text = lr.Text, IsActual = lr.IsActual, IsInEffect = lr.IsInEffect })
                    .ToList()
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

            var caseSummaryDto = new CaseSummaryDTO
            {
                CaseSummaryId = caseSummary.CaseSummaryId,
                Summary = caseSummary.Summary,
                MermaidCode = caseSummary.MermaidCode,
                CaseSummaryWords = caseSummary.GetWords().Select(word => word.Text).ToList(),
                CaseSummaryLegalReferences = caseSummary.GetLegalReferences()
                    .Select(lr => new LegalReferenceStatus { Text = lr.Text, IsActual = lr.IsActual, IsInEffect = lr.IsInEffect })
                    .ToList()
            };

            return View(caseSummaryDto);
        }

       

    }
}