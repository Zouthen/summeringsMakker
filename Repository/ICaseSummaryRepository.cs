namespace summeringsMakker.Repository;

public interface ICaseSummaryRepository
{
    HashSet<string> GetCaseSummariesIds(List<string> periodCaseIds);
}