using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using summeringsmakker.Models;

namespace summeringsmakker.Data
{
    public class CaseDbContext : DbContext
    {
        // specify the tables in the database
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseSummary> CaseSummaries { get; set; }

        public CaseDbContext(DbContextOptions<CaseDbContext> options) : base(options)
        {
        }   
    }
}