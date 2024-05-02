using summeringsmakker.Models;
using summeringsmakker.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using summeringsMakker.Repository;

namespace summeringsmakker.Repository;

public class CaseSummaryRepository : ICaseSummaryRepository
{
    private readonly SummeringsMakkerDbContext _context;

    public CaseSummaryRepository(SummeringsMakkerDbContext context)
    {
        _context = context;
    }

    public CaseSummary GetCaseSummary(int id)
    {
        return _context.CaseSummaries.Find(id);
    }
    
    public List<CaseSummary> GetCaseSummaries()
    {
        return _context.CaseSummaries
            .Include(cs => cs.CaseSummaryWords)
            .ThenInclude(csw => csw.Word)
            .Include(cs => cs.CaseSummaryLegalReferences)
            .ThenInclude(cslr => cslr.LegalReference)
            .ToList();
    }

    public void AddCaseSummary(CaseSummary caseSummary)
    {
        _context.CaseSummaries.Add(caseSummary);
        _context.SaveChanges();
    }

    public void AddCaseSummaryWithReferences(CaseSummary caseSummary)
    {
        foreach (var caseSummaryWord in caseSummary.CaseSummaryWords)
        {
            var word = _context.Words.FirstOrDefault(w => w.Text == caseSummaryWord.Word.Text);
            if (word != null)
            {
                caseSummaryWord.Word = word;
            }
        }

        foreach (var caseSummaryLegalReference in caseSummary.CaseSummaryLegalReferences)
        {
            var legalReference =
                _context.LegalReferences.FirstOrDefault(lr => lr.Text == caseSummaryLegalReference.LegalReference.Text);
            if (legalReference != null)
            {
                caseSummaryLegalReference.LegalReference = legalReference;
            }
        }

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

    public HashSet<string> GetCaseSummariesIds(List<string> periodCaseIds)
    {
        HashSet<string> Ids = new HashSet<string>();

        periodCaseIds.ForEach(periodId => { Ids.Add(periodId); });

        return Ids;
    }

    public void Add(List<CaseSummary> caseSummaries)
    {
        throw new NotImplementedException();
    }
}