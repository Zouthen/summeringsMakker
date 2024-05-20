using summeringsmakker.Models;
using summeringsmakker.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using summeringsMakker.Repository;

namespace summeringsmakker.Repository;

public class CaseSummaryRepository(SummeringsMakkerDbContext context) : ICaseSummaryRepository
{
    public CaseSummary GetCaseSummary(int id)
    {
        return context.CaseSummaries.Find(id);
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