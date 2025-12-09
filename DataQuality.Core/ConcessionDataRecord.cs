namespace DataQuality.Core
{
    public record ConcessionDataRecord(
        string ConcessionName,
        string CompanyName,
        string? CveNumber, 
        string Region,
        float SentimentScore
    );
}