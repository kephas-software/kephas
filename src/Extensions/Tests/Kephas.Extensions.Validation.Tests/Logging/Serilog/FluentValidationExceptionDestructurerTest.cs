// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentValidationExceptionDestructurerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using FluentValidation.Results;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Kephas.Extensions.Tests.Logging.Serilog;

public class FluentValidationExceptionDestructurerTest : IntegrationTestBase<FluentValidationExceptionDestructurerTest>
{
    [Fact]
    public void Destructure_when_validation_exception_logged_then_destructure_successful()
    {
        // Arrange
        var logger = ServiceProvider.GetRequiredService<ILogger<FluentValidationExceptionDestructurerTest>>();
        
        // Act
        Test.UsingCulture("en-US", () =>
        {
            logger.LogWarning(
                new FluentValidation.ValidationException([
                            new ValidationFailure("p1", "bad value", 12)
                        ]),
                "validation failed");
        });
        
        // Assert
        var log = LogBuilder.ToString(); 
        log.Should().StartWith("Warning validation failed");
        log.Should().Contain(" SourceContext,ThreadId,ProcessId,MachineName,ExceptionDetail");
        log.Should().Contain("[(\"Type\": \"FluentValidation.ValidationException\"), (\"HResult\": -2146233088), (\"Message\": \"Validation failed:");
        log.Should().Contain("-- p1: bad value Severity: Error\"), (\"Source\": null), (\"Errors\": [{ PropertyName: \"p1\", AttemptedValue: \"12\", ErrorMessage: \"bad value\", ErrorMessageArgs: null, ErrorCode: null, Severity: Error }])]");
    }

    [Fact]
    public void Destructure_when_validation_exception_logged_then_destructure_successful_DE()
    {
        // Arrange
        
        // Act
        Test.UsingCulture("de-DE", () =>
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<FluentValidationExceptionDestructurerTest>>();

            logger.LogWarning(
                new FluentValidation.ValidationException([
                    new ValidationFailure("p1", "bad value", 12)
                ]),
                "validation failed");
        });
        
        // Assert
        var log = LogBuilder.ToString(); 
        log.Should().StartWith("Warning validation failed");
        log.Should().Contain(" SourceContext,ThreadId,ProcessId,MachineName,ExceptionDetail");
        log.Should().Contain("[(\"Type\": \"FluentValidation.ValidationException\"), (\"HResult\": -2146233088), (\"Message\": \"Validation failed:");
        log.Should().Contain("-- p1: bad value Severity: Error\"), (\"Source\": null), (\"Errors\": [{ PropertyName: \"p1\", AttemptedValue: \"12\", ErrorMessage: \"bad value\", ErrorMessageArgs: null, ErrorCode: null, Severity: Error }])]");
    }
}