using summeringsmakker.Models;
using DBDocument = summeringsmakker.Models.DBDocument;

namespace summeringsmakker.Services;

public interface IDatabaseService
{
    Task<DBDocument> GetFile(string id);
    Task SaveFile(DBDocument file);

    Task<List<DBDocument>> GetFilesFromPeriod(DateTime startDate, DateTime endDate);
}