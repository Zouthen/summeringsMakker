using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using summeringsmakker.DTOs;
using System;

namespace summeringsmakker.Controllers
{
    [ApiController]
    public class CaseSummaryController : Controller
    {
        private readonly ILogger<CaseSummaryController> _logger;

        public CaseSummaryController(ILogger<CaseSummaryController> logger)
        {
            _logger = logger;
        }

        // public IActionResult Index()
        // {
        //     return View();
        // }
        
        [HttpPost("create-case-summaries")]
        public IActionResult CreateCaseSummaries()
        {
            // specify period to whole day
            DateTime startOfDay = DateTime.Today;
            DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            // get from datawarehouse id of all cases for today


            // dont include cases we already have


            // create summaries


            // upload summaries to database






            // Handle the case where neither start nor end date is specified
            // Return an error response or perform appropriate action
            return BadRequest("Both start and end date are required.");
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
