using System.Text;

namespace summeringsmakker.Services;

using summeringsmakker.Models;

public class FakeDatabaseService : IDatabaseService
{
    private string _dataPath;
    
    public async Task<Case> GetFile(string id)
    {
        var files = Directory.GetFiles(_dataPath, "*.txt");
        if (files.Length == 0)
        {
            throw new FileNotFoundException("No .txt files found in the FakeData folder.");
        }

        var fileToRead = files[id.GetHashCode() % files.Length];
        var contentBytes = await System.IO.File.ReadAllBytesAsync(fileToRead);
        var content = Encoding.UTF8.GetString(contentBytes);
        
        return new Case { Id = id, Content = content };
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