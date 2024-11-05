// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationServiceCollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Tests.UseCases.Model;
using Kephas.Extensions.Tests.UseCases.Validators;
using Kephas.Extensions.Validation;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Extensions.Tests;

public class ValidationServiceRegistrationIntegrationTest
{
    [Fact]
    public void AddValidationServices_integration_Validate()
    {
        var validationService = GetValidationService();

        var result = Test.UsingCulture(
            "en-US",
            () => validationService.Validate(new TestPerson("A", "no email", 240, new DateTime(2050, 12, 1))));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Satisfy(
            e => e.ErrorMessage == "'Age' must be between 18 and 130. You entered 240.",
            e => e.ErrorMessage == "The field Name must be a string or array type with a minimum length of '2'.",
            e => e.ErrorMessage == "The Email field is not a valid e-mail address.",
            e => e.ErrorMessage == "'Birthday' must be less than or equal to '4/5/2010 12:00:00 AM'.");
    }

    [Fact]
    public async Task AddValidationServices_integration_ValidateAsync()
    {
        var validationService = GetValidationService();

        var result = await Test.UsingCultureAsync(
            "en-US",
            () => validationService.ValidateAsync(new TestPerson("A", "no email", 240, new DateTime(2050, 12, 1))));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Satisfy(
            e => e.ErrorMessage == "'Age' must be between 18 and 130. You entered 240.",
            e => e.ErrorMessage == "The field Name must be a string or array type with a minimum length of '2'.",
            e => e.ErrorMessage == "The Email field is not a valid e-mail address.",
            e => e.ErrorMessage == "'Birthday' must be less than or equal to '4/5/2010 12:00:00 AM'.");
    }

    [Fact]
    public async Task AddValidationServices_integration_async_validators()
    {
        var serviceProvider = GetServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();

        var result = await Test.UsingCultureAsync(
            "en-US",
            () => validationService.ValidateAsync(new TestBot("Motorola", new DateTime(2050, 12, 1))));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Satisfy(
            e => e.ErrorMessage == "Producer validation failed.",
            e => e.ErrorMessage == "'Birthday' must be less than or equal to '4/5/2010 12:00:00 AM'.");
    }

    private static IValidationService GetValidationService()
    {
        var serviceProvider = GetServiceProvider();
        var validationService = serviceProvider.GetRequiredService<IValidationService>();
        return validationService;
    }

    private static ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddApplicationServices(
            Test.GetIntegrationConfig(),
            options =>
            {
                options.AppAssemblyNamePattern = "^Kephas.";
                options.EntryAssembly = typeof(TestPersonValidator).Assembly;
            });

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }
}
