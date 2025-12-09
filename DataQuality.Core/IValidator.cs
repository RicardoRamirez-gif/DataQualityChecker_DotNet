using System.Collections.Generic;
using System.Threading.Tasks; // <-- Importante para la Asincronía

namespace DataQuality.Core
{
    /// <summary>
    /// The core validation contract (Interface) for all data quality rules.
    /// It uses asynchronous pattern (Task) to allow for non-blocking execution, 
    /// which is crucial for scalable, high-volume backend systems (like DraftKings)
    /// where validation might require external API lookups or database checks.
    /// </summary>
    /// <typeparam name="T">The type of the data record to be validated.</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Asynchronously validates a specific record against the business rule.
        /// </summary>
        /// <param name="record">The data record instance to check.</param>
        /// <returns>A Task representing the asynchronous operation, yielding a validation tuple.</returns>
        Task<(bool IsValid, List<string> Errors)> ValidateAsync(T record);
        
        // OPCIONAL: Podrías añadir un nombre de regla para logging
        // string RuleName { get; }
    }
}