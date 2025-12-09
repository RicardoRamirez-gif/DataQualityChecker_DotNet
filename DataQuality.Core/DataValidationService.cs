using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Necesario para SelectMany
using DataQuality.Core;

namespace DataQuality.Core
{
    /// <summary>
    /// Service responsible for orchestrating the execution of multiple IValidator rules.
    /// This demonstrates the principle of Dependency Injection (DI) and loose coupling
    /// by accepting a collection of rules (IValidator) in the constructor.
    /// </summary>
    public class DataValidationService
    {
        // El servicio depende de una colección de interfaces (IValidator), no de clases concretas.
        private readonly IEnumerable<IValidator<ConcessionDataRecord>> _validators;

        // Inyección de Dependencia (DI): Las reglas se pasan al constructor.
        public DataValidationService(IEnumerable<IValidator<ConcessionDataRecord>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Executes all injected validation rules asynchronously and consolidates the results.
        /// </summary>
        /// <param name="record">The record to be validated.</param>
        /// <returns>A tuple indicating overall validity and a list of all errors found.</returns>
        public async Task<(bool IsValid, List<string> Errors)> ValidateRecordAsync(ConcessionDataRecord record)
        {
            // Colecciona todas las tareas de validación.
            var tasks = new List<Task<(bool IsValid, List<string> Errors)>>();

            // Inicia la ejecución de cada validador en paralelo.
            foreach (var validator in _validators)
            {
                tasks.Add(validator.ValidateAsync(record));
            }

            // Espera a que todas las validaciones terminen (Task.WhenAll).
            var results = await Task.WhenAll(tasks);

            // Consolida y aplana los errores de todos los resultados.
            var errors = results
                .Where(r => !r.IsValid) // Filtra solo los resultados que tienen errores
                .SelectMany(r => r.Errors) // Aplanamos la lista de listas de errores
                .ToList();

            return (errors.Count == 0, errors);
        }
    }
}