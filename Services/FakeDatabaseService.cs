namespace summeringsmakker.Services;

using summeringsmakker.Models;

public class FakeDatabaseService : IDatabaseService
{
    private string _dataPath;
    
    public async Task<DBDocument> GetFile(string id)
    {
        var files = Directory.GetFiles(_dataPath, "*.txt");
        if (files.Length == 0)
        {
            throw new FileNotFoundException("No .txt files found in the FakeData folder.");
        }

        var fileToRead = files[id.GetHashCode() % files.Length];
        var content = await System.IO.File.ReadAllBytesAsync(fileToRead);

        return new DBDocument { Id = id, Content = content };
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