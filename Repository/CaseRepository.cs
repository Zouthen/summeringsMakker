using System.Text;
using summeringsmakker.Models;

namespace summeringsmakker.Repository;

public class CaseRepository : ICaseRepository
{
    public List<Case> GetCases(DateTime startOfDay, DateTime endOfDay)
    {
        var cases = new List<Case>();

        cases.Add(new Case
        {
            Id = "1",
            Content = Encoding.UTF8.GetBytes("This is a case")
        });

        return cases;
    }
}