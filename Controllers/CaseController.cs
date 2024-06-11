using Microsoft.AspNetCore.Mvc;
using System.Text;
using summeringsmakker.Models;
using summeringsmakker.Repository;

namespace summeringsmakker.Controllers;

[ApiController]
[Route("Case")]
public class CaseController(ICaseRepository caseRepository) : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<int> GetCaseById(int id)
    {
        Case c = caseRepository.GetById(id);
        return Ok(c);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Case>> ReadAllDocuments()
    {
        var cases = caseRepository.GetAll(DateTime.MinValue, DateTime.MaxValue);
        return Ok(cases);
    }
}