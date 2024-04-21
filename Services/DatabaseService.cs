using DBDocument = summeringsmakker.Models.DBDocument;

namespace summeringsmakker.Services;

public class DatabaseService : IDatabaseService
{
    public Task<DBDocument> GetFile(string id)
    {
        throw new NotImplementedException();
    }

    public Task SaveFile(DBDocument file)
    {
        throw new NotImplementedException();
    }

    public Task<List<DBDocument>> GetFilesFromPeriod(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }
}