using System.Collections.Generic;
using System.Threading.Tasks; // <-- NUEVO: Para usar Task
using DataQuality.Core; 
using System.Text.RegularExpressions;

namespace DataQuality.Core
{
    public class SentimentScoreValidator : IValidator<ConcessionDataRecord>
    {
        private const float MinScore = -2.0f;
        private const float MaxScore = 2.0f;

        // MODIFICACIÓN CLAVE: Cambiar a ValidateAsync y devolver Task<T>
        public Task<(bool IsValid, List<string> Errors)> ValidateAsync(ConcessionDataRecord record)
        {
            var errors = new List<string>();
            float score = record.SentimentScore; 

            // Rule: Check if the score is outside the allowed range.
            if (score < MinScore || score > MaxScore)
            {
                errors.Add($"Sentiment Score ({score}) is outside the required range of {MinScore} to {MaxScore}.");
            }

            // Devolver el resultado envuelto en una Task.
            return Task.FromResult((errors.Count == 0, errors));
        }
    }
}