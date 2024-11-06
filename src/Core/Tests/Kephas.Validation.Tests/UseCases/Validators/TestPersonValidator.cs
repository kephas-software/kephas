// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestPersonValidator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentValidation;
using Kephas.Tests.UseCases.Model;

namespace Kephas.Tests.UseCases.Validators;

public class TestPersonValidator : AbstractValidator<TestPerson>
{
    public TestPersonValidator()
    {
        RuleFor(e => e.Age).InclusiveBetween(18, 130);
    }
}
