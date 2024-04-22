using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using summeringsmakker.Data;
using summeringsmakker.Models;
using summeringsmakker.Repository;
using System.Linq;

public class CaseSummaryRepositoryTests
{
    private CaseDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CaseDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Database name needs to be unique per test
            .Options;

        return new CaseDbContext(options);
    }

    [Fact]
    public void GetCaseSummary_ReturnsCorrectCaseSummary()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var caseSummary = new CaseSummary { Id = 1, Summary = "Test Case", MermaidCode = "code"};
        dbContext.CaseSummaries.Add(caseSummary);
        dbContext.SaveChanges();

        var repository = new CaseSummaryRepository(dbContext);

        // Act
        var result = repository.GetCaseSummary(1);

        // Assert
        Assert.Equal(caseSummary, result);
    }

    [Fact]
    public void AssertFalse()
    {
        Assert.Equal(true, true);
    }
}