using summeringsmakker.Models;
using summeringsmakker.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using summeringsMakker.Repository;

namespace summeringsmakker.Repository;

public class CaseSummaryRepository(SummeringsMakkerDbContext context) : ICaseSummaryRepository
{
    public CaseSummary GetById(int id)
    {
        return context.CaseSummaries
        .Include(cs => cs.CaseSummaryWords)
            .ThenInclude(csw => csw.Word)
        .Include(cs => cs.CaseSummaryLegalReferences)
            .ThenInclude(cslr => cslr.LegalReference)
        .FirstOrDefault(cs => cs.CaseSummaryId == id);
    }
    
    public List<CaseSummary> GetCaseSummaries()
    {
        return context.CaseSummaries
            .Include(cs => cs.CaseSummaryWords)
            .ThenInclude(csw => csw.Word)
            .Include(cs => cs.CaseSummaryLegalReferences)
            .ThenInclude(cslr => cslr.LegalReference)
            .ToList();
    }

    public void AddCaseSummary(CaseSummary caseSummary)
    {
        context.CaseSummaries.Add(caseSummary);
        context.SaveChanges();
    }

    public void AddCaseSummaryWithReferences(CaseSummary caseSummary)
    {
        foreach (var caseSummaryWord in caseSummary.CaseSummaryWords)
        {
            var word = context.Words.FirstOrDefault(w => w.Text == caseSummaryWord.Word.Text);
            if (word != null)
            {
                caseSummaryWord.Word = word;
            }
        }

        foreach (var caseSummaryLegalReference in caseSummary.CaseSummaryLegalReferences)
        {
            var legalReference =
                context.LegalReferences.FirstOrDefault(lr => lr.Text == caseSummaryLegalReference.LegalReference.Text);
            if (legalReference != null)
            {
                caseSummaryLegalReference.LegalReference = legalReference;
            }
        }

        context.CaseSummaries.Add(caseSummary);
        context.SaveChanges();
    }

    public void UpdateCaseSummary(CaseSummary caseSummary)
    {
        context.CaseSummaries.Update(caseSummary);
        context.SaveChanges();
    }

    public void DeleteCaseSummary(int id)
    {
        var caseSummary = context.CaseSummaries.Find(id);
        if (caseSummary != null)
        {
            context.CaseSummaries.Remove(caseSummary);
            context.SaveChanges();
        }
    }
    
    public HashSet<int> GetCaseIds(List<int> periodCaseIds)
    {
        var ids = context.CaseSummaries
                        .AsNoTracking()
                        .Where(cs => periodCaseIds.Contains(cs.CaseId)) // Ensure cs.CaseId is the intended field
                        .Select(cs => cs.CaseId) // This selects the CaseId from those filtered entries
                        .ToHashSet();
        return ids;
    }
      /*
    public HashSet<int> GetCaseIds(List<int> periodCaseIds)
    {
        // Parameterize the input to prevent SQL injection
        var ids = context.CaseSummaries
                        .FromSqlRaw("SELECT CaseId FROM CaseSummaries WHERE CaseId IN ({0})", periodCaseIds)
                        .AsEnumerable()
                        .ToHashSet();
        return ids;
    }
*/

    public void Add(List<CaseSummary> caseSummaries)
    {
        if (caseSummaries == null || !caseSummaries.Any())
        {
            throw new ArgumentNullException(nameof(caseSummaries), "The list of case summaries cannot be null or empty.");
        }

        context.CaseSummaries.AddRange(caseSummaries);
        context.SaveChanges();
    }

}