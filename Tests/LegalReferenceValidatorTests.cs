using Xunit;
using summeringsMakker.Services;
using summeringsmakker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using summeringsmakker.Data;
using summeringsmakker.Repository;

// fixture to share the same instance of LegalReferenceValidator for all tests
public class LegalReferenceValidatorFixture
{
    public LegalReferenceValidator Validator { get; private set; }

    public LegalReferenceValidatorFixture()
    {
        var options = new DbContextOptionsBuilder<SummeringsMakkerDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new SummeringsMakkerDbContext(options);
        var caseRepository = new Mock<CaseRepository>();

        Validator = new LegalReferenceValidator(context, caseRepository.Object);
    }
}

public class LegalReferenceValidatorTests(LegalReferenceValidatorFixture fixture)
    : IClassFixture<LegalReferenceValidatorFixture>
{
    private LegalReferenceValidator _validator = fixture.Validator;

    [Fact]
    public async Task ValidateLegalReferences_ShouldReturnExpectedResults()
    {
        // Arrange
        CaseSummary caseSummary = new CaseSummary
        {
            CaseSummaryId = 1,
            CaseId = 10,
            Summary = "Summary",
            MermaidCode = "MermaidCode",
            LastChecked = DateTime.Now,
        };
        CaseSummaryLegalReference caseSummaryLegalReference = new CaseSummaryLegalReference
        {
            CaseSummaryId = 1,
            CaseSummary = caseSummary,
            LegalReferenceId = 1,
            LegalReference = new LegalReference()
        };
        caseSummary.CaseSummaryLegalReferences.Add(caseSummaryLegalReference);
        
        // Act
        var caseSummaryResult = await _validator.ValidateCaseSummaryLegalReferences(caseSummary);
        
        // Assert
        bool isValidExpected = true;
        bool IsInEffectExpected = true;
        
        Assert.Equal(isValidExpected, caseSummaryResult.CaseSummaryLegalReferences[0].LegalReference.IsActual);
        Assert.Equal(IsInEffectExpected, caseSummaryResult.CaseSummaryLegalReferences[0].LegalReference.IsInEffect);
    }
}