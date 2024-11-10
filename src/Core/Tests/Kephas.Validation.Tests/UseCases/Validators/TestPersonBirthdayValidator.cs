// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestPersonValidator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentValidation;
using Kephas.Tests.UseCases.Model;

namespace Kephas.Tests.UseCases.Validators;

public class TestPersonBirthdayValidator : AbstractValidator<IPerson>
{
    public TestPersonBirthdayValidator()
    {
        RuleFor(e => e.Birthday).LessThanOrEqualTo(new DateTime(2010, 4, 5));
    }
}
