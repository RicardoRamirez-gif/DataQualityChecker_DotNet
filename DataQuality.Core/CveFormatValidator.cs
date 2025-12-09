// DataQuality.Core/CveFormatValidator.cs (y SentimentScoreValidator.cs)
using System.Collections.Generic;
using System.Threading.Tasks; 
using DataQuality.Core; // <--- Línea crucial para auto-referencia
using System.Text.RegularExpressions; // Solo en CveFormatValidator

namespace DataQuality.Core
{
    public class CveFormatValidator : IValidator<ConcessionDataRecord> 
    {
        private static readonly Regex CveRegex = new Regex(@"^\d{5}$", RegexOptions.Compiled);

        // MODIFICACIÓN CLAVE: Cambiar a ValidateAsync y devolver Task<T>
        public Task<(bool IsValid, List<string> Errors)> ValidateAsync(ConcessionDataRecord record)
        {
            var errors = new List<string>();

            // Regla 1: Integridad Check (Non-Null/Whitespace)
            if (string.IsNullOrWhiteSpace(record.CveNumber))
            {
                errors.Add("CVE Number cannot be empty or null.");
            }
            // Regla 2: Format Check (Must match the 5-digit regex pattern)
            else if (!CveRegex.IsMatch(record.CveNumber))
            {
                errors.Add($"CVE Number '{record.CveNumber}' is not in the required 5-digit format (e.g., 12345).");
            }

            // Dado que esta lógica es síncrona, usamos Task.FromResult para envolver el resultado.
            return Task.FromResult((errors.Count == 0, errors));
        }
    }
}