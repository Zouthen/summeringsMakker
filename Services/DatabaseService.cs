using summeringsmakker.Models;

namespace summeringsmakker.Services;

public class DatabaseService : IDatabaseService
{
    public Task<Case> GetFile(string id)
    {
        throw new NotImplementedException();
    }

    public Task SaveFile(Case file)
    {
        throw new NotImplementedException();
    }

    public Task<List<Case>> GetFilesFromPeriod(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }
}