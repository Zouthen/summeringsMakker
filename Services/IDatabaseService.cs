using summeringsmakker.Models;

namespace summeringsmakker.Services;

public interface IDatabaseService
{
    Task<Case> GetFile(string id);
    Task SaveFile(Case file);

    Task<List<Case>> GetFilesFromPeriod(DateTime startDate, DateTime endDate);
}