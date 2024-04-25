using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using summeringsmakker.Models;

namespace summeringsmakker.Data;

public class SummeringsMakkerDbContext : DbContext
{
    public DbSet<CaseSummary> CaseSummaries { get; set; }
    public DbSet<Word> Words { get; set; }
    public DbSet<CaseSummaryWord> CaseSummaryWords { get; set; }
    public DbSet<LegalReference> LegalReferences { get; set; }
    public DbSet<CaseSummaryLegalReference> CaseSummaryLegalReferences { get; set; }

    public SummeringsMakkerDbContext(DbContextOptions<SummeringsMakkerDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CaseSummaryWord>()
            .HasKey(csw => new { csw.CaseSummaryId, csw.WordId });

        modelBuilder.Entity<CaseSummaryWord>()
            .HasOne(csw => csw.CaseSummary)
            .WithMany(cs => cs.CaseSummaryWords)
            .HasForeignKey(csw => csw.CaseSummaryId);

        modelBuilder.Entity<CaseSummaryWord>()
            .HasOne(csw => csw.Word)
            .WithMany(w => w.CaseSummaryWords)
            .HasForeignKey(csw => csw.WordId);

        modelBuilder.Entity<CaseSummaryLegalReference>()
            .HasKey(cslr => new { cslr.CaseSummaryId, cslr.LegalReferenceId });

        modelBuilder.Entity<CaseSummaryLegalReference>()
            .HasOne(cslr => cslr.CaseSummary)
            .WithMany(cs => cs.CaseSummaryLegalReferences)
            .HasForeignKey(cslr => cslr.CaseSummaryId);

        modelBuilder.Entity<CaseSummaryLegalReference>()
            .HasOne(cslr => cslr.LegalReference)
            .WithMany(lr => lr.CaseSummaryLegalReferences)
            .HasForeignKey(cslr => cslr.LegalReferenceId);
    }
}