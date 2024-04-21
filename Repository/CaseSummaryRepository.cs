using summeringsmakker.Models;
using summeringsmakker.Data;
using System.Linq;

namespace summeringsmakker.Repository;

public class CaseSummaryRepository
{
    private readonly CaseDbContext _context;

    public CaseSummaryRepository(CaseDbContext context)
    {
        _context = context;
    }

    public CaseSummary GetCaseSummary(int id)
    {
        return _context.CaseSummaries.Find(id);
    }

    public List<CaseSummary> GetCaseSummaries()
    {
        return _context.CaseSummaries.ToList();
    }

    public void AddCaseSummary(CaseSummary caseSummary)
    {
        _context.CaseSummaries.Add(caseSummary);
        _context.SaveChanges();
    }

    public void UpdateCaseSummary(CaseSummary caseSummary)
    {
        _context.CaseSummaries.Update(caseSummary);
        _context.SaveChanges();
    }

    public void DeleteCaseSummary(int id)
    {
        var caseSummary = _context.CaseSummaries.Find(id);
        if (caseSummary != null)
        {
            _context.CaseSummaries.Remove(caseSummary);
            _context.SaveChanges();
        }
    }
}