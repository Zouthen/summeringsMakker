namespace summeringsmakker.Models;

public interface ICaseRepository
{
    List<Case> GetCases(DateTime startOfDay, DateTime endOfDay);
}