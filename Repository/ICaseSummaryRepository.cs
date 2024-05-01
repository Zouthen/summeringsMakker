using summeringsmakker.Models;

namespace summeringsMakker.Repository;

public interface ICaseSummaryRepository
{
    HashSet<string> GetCaseSummariesIds(List<string> periodCaseIds);
    void Add(List<CaseSummary> caseSummaries);

    public List<CaseSummary> GetCaseSummaries();
    public List<CaseSummary> GetCaseSummaries2();

    public void AddCaseSummary(CaseSummary caseSummary);
}