using summeringsmakker.Models;

namespace summeringsMakker.Repository;

public interface ICaseSummaryRepository
{
    HashSet<int> GetCaseIds(List<int> periodCaseIds);
    void Add(List<CaseSummary> caseSummaries);


    public CaseSummary GetById(int id);
    public List<CaseSummary> GetCaseSummaries();
    public void AddCaseSummary(CaseSummary caseSummary);
}