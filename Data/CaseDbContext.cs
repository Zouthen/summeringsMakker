using Microsoft.EntityFrameworkCore;
using summeringsmakker.Models;

namespace summeringsmakker.Data;

public class CaseDbContext(DbContextOptions<CaseDbContext> options) : DbContext(options)
{
    // specify the tables in the database
    public DbSet<Case> Cases { get; set; }
    public DbSet<CaseSummary> CaseSummaries { get; set; }
    
    // specify the connection string
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=summeringsmakker;Trusted_Connection=True;");
    }
}