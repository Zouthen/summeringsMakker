using summeringsmakker.Models;

namespace summeringsMakker.Repository;

public interface ICaseSummaryRepository
{
    HashSet<string> GetCaseSummariesIds(List<string> periodCaseIds);
    void Add(List<CaseSummary> caseSummaries);


    public CaseSummary GetById(string id);
    public List<CaseSummary> GetCaseSummaries();
    public void AddCaseSummary(CaseSummary caseSummary);
}