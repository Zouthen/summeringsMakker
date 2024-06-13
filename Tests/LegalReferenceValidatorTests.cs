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
        // Initialize the LegalReferenceValidator object
        // Mock the necessary dependencies
        var options = new DbContextOptionsBuilder<SummeringsMakkerDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Create a new in-memory database for testing
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
        Assert.True(true, "Test not implemented yet");


        // Arrange
        CaseSummary caseSummary = new CaseSummary
        {
            CaseSummaryId = 1,
            CaseId = 1,
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
        
        var reference = DeepCopy(caseSummaryLegalReference);
        
        // Act
        var resultCaseSummary = await _validator.ValidateCaseSummaryLegalReferences(caseSummary);

        // Assert
        Assert.NotNull(resultCaseSummary);
        for (int i = 0; i < resultCaseSummary.CaseSummaryLegalReferences.Count; i++)
        {
            CaseSummaryLegalReference cs = caseSummary.CaseSummaryLegalReferences[i];
            CaseSummaryLegalReference csExpected = reference.CaseSummary.CaseSummaryLegalReferences[i];
            Assert.Equal(csExpected.CaseSummaryId, cs.CaseSummaryId);
        }
    }
    
    // helper methods
    public CaseSummaryLegalReference DeepCopy(CaseSummaryLegalReference original)
    {
        return new CaseSummaryLegalReference
        {
            CaseSummaryId = original.CaseSummaryId,
            CaseSummary = new CaseSummary
            {
                CaseSummaryId = original.CaseSummary.CaseSummaryId,
                CaseId = original.CaseSummary.CaseId,
                Summary = original.CaseSummary.Summary,
                MermaidCode = original.CaseSummary.MermaidCode,
                LastChecked = original.CaseSummary.LastChecked,
                // Copy other properties of CaseSummary if there are any
            },
            LegalReferenceId = original.LegalReferenceId,
            LegalReference = new LegalReference
            {
                LegalReferenceId = original.LegalReference.LegalReferenceId,
                // Copy other properties of LegalReference if there are any
            }
        };
    }
    
}