// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestPersonValidator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentValidation;
using Kephas.Extensions.Tests.UseCases.Model;
using Kephas.Extensions.Tests.UseCases.Services;

namespace Kephas.Extensions.Tests.UseCases.Validators;

public class TestBotRepositoryValidator : AbstractValidator<TestBot>
{
    public TestBotRepositoryValidator(IRepository repository)
    {
        RuleFor(e => e.Producer)
            .MustAsync(async (name, token) => name == (await repository.GetAsync<Rule>(token)).Name)
            .WithMessage("Producer validation failed.");
    }
}
