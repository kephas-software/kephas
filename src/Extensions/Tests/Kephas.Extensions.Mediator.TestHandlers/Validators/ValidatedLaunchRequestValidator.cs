// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatedLaunchRequestValidator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentValidation;

namespace Kephas.Extensions.Mediator.TestHandlers.Validators;

public class ValidatedLaunchRequestValidator : AbstractValidator<ValidatedLaunchRequest>
{
    public ValidatedLaunchRequestValidator()
    {
        RuleFor(r => r.Command)
            .NotEmpty();
    }
}
