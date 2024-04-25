namespace summeringsmakker.Controllers;

using System;
using Microsoft.AspNetCore.Mvc;
using summeringsmakker.Models;
using summeringsMakker.Services;

public class DocumentController : Controller
{
    public async Task<IActionResult> Process()
    {
        //var viewModel = new TextProcessed();
        var viewModelCase = new CaseSummary();
        string filePath = "afgørelse.pdf";
        // assuming ProcessFile now returns a ViewModel instead of just a string
        //viewModel = await TextProcessor.ProcessFile(filePath);
        viewModelCase = await CaseProcessor.ProcessFile(filePath);

        // return the ViewModel to the view
        return View(viewModelCase);
    }
    /*
    public async IActionResult save()
    {
    

        CaseSummary = new CaseSummary 
        {
            


        }


        // save i db case summary ord
        _caseSummaryRepository.Add(caseSummaries);

    }
    */
}