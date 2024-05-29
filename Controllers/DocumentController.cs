using summeringsmakker.Repository;

namespace summeringsmakker.Controllers;

using System;
using Microsoft.AspNetCore.Mvc;
using summeringsmakker.Models;
using summeringsMakker.Repository;
using summeringsMakker.Services;

public class DocumentController : Controller
{
    private readonly CaseProcessor _caseProcessor;
    private readonly ICaseSummaryRepository _caseSummaryRepository;

    public DocumentController(CaseProcessor caseProcessor, ICaseSummaryRepository caseSummaryRepository)
    {
        _caseProcessor = caseProcessor;
        _caseSummaryRepository = caseSummaryRepository;
    }
    
    // TODO: make pdf path work
    // public async Task<IActionResult> Process()
    // {
    //     var viewModelCase = new CaseSummary();
    //     string filePath = "afgørelse.pdf";
    //     viewModelCase = await _caseProcessor.ProcessFile(filePath);

    //     _caseSummaryRepository.AddCaseSummary(viewModelCase);

        

    //     return View(viewModelCase);
    // }
    
    
}