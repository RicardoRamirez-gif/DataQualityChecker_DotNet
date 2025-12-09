using Xunit;
using System.Linq;
using System.Threading.Tasks; // Required for async Task
using DataQuality.Core;
using System.Collections.Generic;

namespace DataQuality.Tests
{
    // =================================================================
    // CveFormatValidator Tests (Rule 1: Format and Integrity)
    // =================================================================
    public class CveValidatorTests
    {
        // Test 1: Happy Path
        [Fact]
        public async Task Validate_ValidCveNumber_ReturnsTrueAndNoErrors() 
        {
            var validRecord = new ConcessionDataRecord(
                ConcessionName: "Test Name", CompanyName: "Test Co.", CveNumber: "12345", 
                Region: "Test Region", SentimentScore: 1.0f
            );
            var validator = new CveFormatValidator();
            
            // CORRECTED CALL: Use await and ValidateAsync
            var result = await validator.ValidateAsync(validRecord); 

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        // Test 2: Integrity Check (Null/Empty)
        [Theory]
        [InlineData(null)] 
        [InlineData("")]
        [InlineData(" ")]
        public async Task Validate_NullOrEmptyCveNumber_ReturnsFalseAndOneError(string? invalidCve)
        {
            var invalidRecord = new ConcessionDataRecord(
                ConcessionName: "Test Name", CompanyName: "Test Co.", CveNumber: invalidCve, 
                Region: "Test Region", SentimentScore: 1.0f
            );
            var validator = new CveFormatValidator();
            
            // CORRECTED CALL: Use await and ValidateAsync
            var result = await validator.ValidateAsync(invalidRecord); 

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
        }
        
        // Test 3: Format Check (Incorrect Length/Chars)
        [Theory]
        [InlineData("1234")] [InlineData("123456")] [InlineData("ABCDE")] 
        public async Task Validate_IncorrectFormatCveNumber_ReturnsFalseAndOneError(string invalidCve)
        {
            var invalidRecord = new ConcessionDataRecord(
                ConcessionName: "Test Name", CompanyName: "Test Co.", CveNumber: invalidCve, 
                Region: "Test Region", SentimentScore: 1.0f
            );
            var validator = new CveFormatValidator();
            
            // CORRECTED CALL: Use await and ValidateAsync
            var result = await validator.ValidateAsync(invalidRecord);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("is not in the required 5-digit format", result.Errors.First());
        }
    }

    // =================================================================
    // SentimentScoreValidator Tests (Rule 2: Range Check)
    // =================================================================
    public class SentimentScoreValidatorTests
    {
        // Test 4: Happy Path
        [Fact]
        public async Task Validate_ScoreWithinRange_ReturnsTrue()
        {
            var record = new ConcessionDataRecord("N", "C", "12345", "R", 1.5f); 
            var validator = new SentimentScoreValidator();
            
            // CORRECTED CALL: Use await and ValidateAsync
            var result = await validator.ValidateAsync(record);
            
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        // Test 5: Boundary/Edge Case Check
        [Theory]
        [InlineData(2.0f)] [InlineData(-2.0f)] [InlineData(0.0f)] 
        public async Task Validate_ScoreAtBoundary_ReturnsTrue(float boundaryScore)
        {
            var record = new ConcessionDataRecord("N", "C", "12345", "R", boundaryScore); 
            var validator = new SentimentScoreValidator();
            
            // CORRECTED CALL: Use await and ValidateAsync
            var result = await validator.ValidateAsync(record);
            
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        // Test 6: Failure Path (Outside Range)
        [Theory]
        [InlineData(2.001f)] [InlineData(-2.001f)] [InlineData(10.0f)] 
        public async Task Validate_ScoreOutsideRange_ReturnsFalse(float invalidScore)
        {
            var record = new ConcessionDataRecord("N", "C", "12345", "R", invalidScore);
            var validator = new SentimentScoreValidator();
            
            // CORRECTED CALL: Use await and ValidateAsync
            var result = await validator.ValidateAsync(record);
            
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
        }
    }
}