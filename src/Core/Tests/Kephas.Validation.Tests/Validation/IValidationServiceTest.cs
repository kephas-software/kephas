// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using FluentValidation.Results;
using Kephas.Validation;
using Xunit;

namespace Kephas.Extensions.Tests.Validation;

public class IValidationServiceTest
{
    [Fact]
    public void EnsureValid_IsValid()
    {
        IValidationService validationService = new TestValidationService();
        var exception = Record.Exception(() => validationService.EnsureValid("hello"));
        exception.Should().BeNull();
    }

    [Fact]
    public async Task EnsureValidAsync_IsValid()
    {
        IValidationService validationService = new TestValidationService();
        var exception = await Record.ExceptionAsync(() => validationService.EnsureValidAsync("hello"));
        exception.Should().BeNull();
    }

    [Fact]
    public void EnsureValid_IsValid_false()
    {
        IValidationService validationService = new TestValidationService(new Exception("wrong data"));
        var ex = Assert.Throws<ValidationException>(() => validationService.EnsureValid("hello"));
        ex.ValidatedInstance.Should().Be("hello");
        ex.ValidationResult.Should().NotBeNull();
        ex.ValidationResult!.Errors.Should().Satisfy(
            e => e.ErrorMessage == "wrong data");
    }

    [Fact]
    public async Task EnsureValidAsync_IsValid_false()
    {
        IValidationService validationService = new TestValidationService(new Exception("wrong data"));
        var ex = await Assert.ThrowsAsync<ValidationException>(() => validationService.EnsureValidAsync("hello"));
        ex.ValidatedInstance.Should().Be("hello");
        ex.ValidationResult.Should().NotBeNull();
        ex.ValidationResult!.Errors.Should().Satisfy(
            e => e.ErrorMessage == "wrong data");
    }

    private class TestValidationService(Exception? exception = null) : IValidationService
    {
        public ValidationResult Validate(object? obj) =>
            exception is null
                ? new ValidationResult()
                : new ValidationResult([new ValidationFailure("PROP", exception.Message)]);

        public Task<ValidationResult> ValidateAsync(object? obj, CancellationToken cancellationToken = default) =>
            Task.FromResult(Validate(obj));
    }
}
