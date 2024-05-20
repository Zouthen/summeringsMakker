namespace summeringsmakker.Models;

public interface ICaseRepository
{
    List<Case> GetAll(DateTime? startOfDay = null, DateTime? endOfDay = null);
    Case GetById(string id);
}