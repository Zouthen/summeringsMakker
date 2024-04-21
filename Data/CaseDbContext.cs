using Microsoft.EntityFrameworkCore;
using summeringsmakker.Models;

namespace summeringsmakker.Data;

public class CaseDbContext : DbContext
{
    public CaseDbContext(DbContextOptions<CaseDbContext> options) : base(options)
    {
    }

    public DbSet<Case> Cases { get; set; }
    public DbSet<CaseSummary> CaseSummaries { get; set; }
}