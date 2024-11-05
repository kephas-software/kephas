// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatorsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Extensions.Validation;

/// <summary>
/// Default implementation of the <see cref="ValidatorsProvider"/>.
/// </summary>
public class ValidatorsProvider(IServiceProvider serviceProvider) : IValidatorsProvider
{
    /// <summary>
    /// Gets an enumeration of validators.
    /// </summary>
    /// <param name="validatedType">The type of the validated instance.</param>
    /// <returns>An enumeration of validators.</returns>
    public IEnumerable<IValidator> GetValidators(Type validatedType)
    {
        if (validatedType is null)
            return Enumerable.Empty<IValidator>();

        var validators = GetValidatorsCore(validatedType).ToList();
        validators.Add(new DataAnnotationsValidator());

        if (validatedType.IsInterface)
            return validators;

        validators.AddRange(validatedType.GetInterfaces().SelectMany(GetValidatorsCore));
        return validators;
    }

    private IEnumerable<IValidator> GetValidatorsCore(Type validatedType)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(validatedType);
        var validatorEnumerableType = typeof(IEnumerable<>).MakeGenericType(validatorType);
        return (IEnumerable<IValidator>)ServiceProviderServiceExtensions.GetRequiredService(serviceProvider, validatorEnumerableType);
    }
}
