using Xunit;
using Moq; // Required for Mocking Framework
using System.Collections.Generic;
using System.Threading.Tasks;
using DataQuality.Core;
using System.Linq;

namespace DataQuality.Tests
{
    // Esta clase prueba la orquestación y la Inyección de Dependencia (DI)
    public class DataValidationServiceTests
    {
        // El Happy Path: Verifica que si todas las reglas pasan, el resultado es True.
        [Fact]
        public async Task ValidateRecordAsync_AllRulesPass_ReturnsTrue()
        {
            // ARRANGE (Preparación de Mocks y Servicio)
            var recordToValidate = new ConcessionDataRecord("N", "C", "12345", "R", 1.0f);
            
            // 1. Mock de Regla de CVE: Simular que PASA
            var cveValidatorMock = new Mock<IValidator<ConcessionDataRecord>>();
            // Setup el método asíncrono del mock para devolver un resultado de PASA
            cveValidatorMock.Setup(v => v.ValidateAsync(recordToValidate))
                            .ReturnsAsync((true, new List<string>()));

            // 2. Mock de Regla de Sentimiento: Simular que PASA
            var sentimentValidatorMock = new Mock<IValidator<ConcessionDataRecord>>();
            sentimentValidatorMock.Setup(v => v.ValidateAsync(recordToValidate))
                                  .ReturnsAsync((true, new List<string>()));
            
            // Colección de validadores inyectados al servicio
            var validators = new List<IValidator<ConcessionDataRecord>>
            {
                cveValidatorMock.Object, // Inyectamos las instancias Mock
                sentimentValidatorMock.Object
            };
            
            var service = new DataValidationService(validators);

            // ACT (Ejecución del servicio orquestador)
            var result = await service.ValidateRecordAsync(recordToValidate);

            // ASSERT (Verificación)
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            
            // Opcional: Verifica que los mocks fueron llamados una vez, confirmando la ejecución DI.
            cveValidatorMock.Verify(v => v.ValidateAsync(recordToValidate), Times.Once);
            sentimentValidatorMock.Verify(v => v.ValidateAsync(recordToValidate), Times.Once);
        }

        // El Failure Path: Verifica que si una regla falla, el resultado es False y los errores se consolidan.
        [Fact]
        public async Task ValidateRecordAsync_OneRuleFails_ConsolidatesErrorsAndReturnsFalse()
        {
            // ARRANGE (Preparación de Mocks con fallo)
            var recordToValidate = new ConcessionDataRecord("N", "C", "INVALID", "R", 1.0f); 
            
            // 1. Mock de Regla de CVE: Simular que FALLA con 1 error.
            var cveValidatorMock = new Mock<IValidator<ConcessionDataRecord>>();
            cveValidatorMock.Setup(v => v.ValidateAsync(recordToValidate))
                            .ReturnsAsync((false, new List<string> { "CVE must be 5 digits." }));

            // 2. Mock de Regla de Sentimiento: Simular que PASA.
            var sentimentValidatorMock = new Mock<IValidator<ConcessionDataRecord>>();
            sentimentValidatorMock.Setup(v => v.ValidateAsync(recordToValidate))
                                  .ReturnsAsync((true, new List<string>())); 
            
            // Colección de validadores inyectados
            var validators = new List<IValidator<ConcessionDataRecord>>
            {
                cveValidatorMock.Object,
                sentimentValidatorMock.Object
            };
            
            var service = new DataValidationService(validators);

            // ACT
            var result = await service.ValidateRecordAsync(recordToValidate);

            // ASSERT
            Assert.False(result.IsValid);
            Assert.Single(result.Errors); // Solo debe haber 1 error (del CVE)
            Assert.Equal("CVE must be 5 digits.", result.Errors.First());
        }

        // El Multi-Failure Path: Verifica que múltiples errores de múltiples reglas se consoliden.
        [Fact]
        public async Task ValidateRecordAsync_MultipleRulesFail_ConsolidatesAllErrors()
        {
            // ARRANGE (Preparación de Mocks con múltiples fallos)
            var recordToValidate = new ConcessionDataRecord("N", "C", "INV", "R", 5.0f);
            
            // 1. Mock de Regla de CVE: Simular que FALLA (1 error)
            var cveValidatorMock = new Mock<IValidator<ConcessionDataRecord>>();
            cveValidatorMock.Setup(v => v.ValidateAsync(recordToValidate))
                            .ReturnsAsync((false, new List<string> { "Error: CVE format invalid." }));

            // 2. Mock de Regla de Sentimiento: Simular que FALLA (2 errores)
            var sentimentValidatorMock = new Mock<IValidator<ConcessionDataRecord>>();
            sentimentValidatorMock.Setup(v => v.ValidateAsync(recordToValidate))
                                  .ReturnsAsync((false, new List<string> { "Error: Score too high.", "Error: Score is critical." }));
            
            // Colección de validadores inyectados
            var validators = new List<IValidator<ConcessionDataRecord>>
            {
                cveValidatorMock.Object,
                sentimentValidatorMock.Object
            };
            
            var service = new DataValidationService(validators);

            // ACT
            var result = await service.ValidateRecordAsync(recordToValidate);

            // ASSERT
            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count); // Verifica que los 3 errores se consolidaron (1 + 2)
            Assert.Contains("Error: CVE format invalid.", result.Errors);
            Assert.Contains("Error: Score too high.", result.Errors);
        }
    }
}