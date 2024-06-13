using System.Text;
using summeringsmakker.Models;

namespace summeringsmakker.Repository;


public class CaseRepository : ICaseRepository
{
    private const string Path = "Data/Arvid/";

    public List<Case> GetAll(DateTime? startOfDay = null, DateTime? endOfDay = null)
    {
        var cases = new List<Case>();
        var files = Directory.EnumerateFiles(Path);
        
        foreach (var file in files)
        {
            var id = System.IO.Path.GetFileNameWithoutExtension(file);
            var content = File.ReadAllText(file);
        
            cases.Add(new Case
            {
                Id = int.Parse(id),
                Content = (content)
            });
        }

        return cases;
    }

    public Case GetById(int id)
    {
        string binPath = AppDomain.CurrentDomain.BaseDirectory;
        string projectRootPath = Directory.GetParent(binPath).Parent.Parent.Parent.FullName;
        string filePath = System.IO.Path.Combine(projectRootPath, Path, $"{id}.txt");

        if (!File.Exists(filePath))
        {
            return null;
        }

        var content = File.ReadAllText(filePath, Encoding.UTF8);
        return new Case
        {
            Id = id,
            Content = content
        };
    }
}