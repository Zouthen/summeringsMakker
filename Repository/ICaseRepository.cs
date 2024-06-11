using summeringsmakker.Models;

namespace summeringsmakker.Repository;

public interface ICaseRepository
{
    List<Case> GetAll(DateTime? startOfDay = null, DateTime? endOfDay = null);
    Case GetById(int id);
}
