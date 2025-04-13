// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationExceptionDestructurerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using FluentValidation.Results;
using Kephas.Testing;
using Kephas.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Kephas.Extensions.Tests.Logging.Serilog;

public class ValidationExceptionDestructurerTest : IntegrationTestBase<ValidationExceptionDestructurerTest>
{
    [Fact]
    public void Destructure_when_validation_exception_logged_then_destructure_successful()
    {
        // Arrange
        var logger = ServiceProvider.GetRequiredService<ILogger<ValidationExceptionDestructurerTest>>();
        
        // Act
        Test.UsingCulture("en-US", () =>
        {
            logger.LogWarning(
                new ValidationException(
                    validationResult: new ValidationResult
                    {
                        Errors = [
                            new ValidationFailure("p1", "bad value", 12)
                        ]
                    }),
                "validation failed");
        });
        
        // Assert
        var log = LogBuilder.ToString(); 
        log.Should().StartWith("Warning validation failed");
        log.Should().Contain(" SourceContext,ThreadId,ProcessId,MachineName,ExceptionDetail");
        log.Should().Contain("[(\"Type\": \"Kephas.Validation.ValidationException\"), (\"HResult\": -2146233088), (\"Message\": \"The provided input is not valid. Please check the validation result for more information.\"), (\"Source\": null), (\"ValidationResult\": { IsValid: False, Errors: [{ PropertyName: \"p1\", AttemptedValue: \"12\", ErrorMessage: \"bad value\", ErrorMessageArgs: null, ErrorCode: null, Severity: Error }] })]");
    }
}