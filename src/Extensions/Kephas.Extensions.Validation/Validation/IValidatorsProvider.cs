// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidatorsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentValidation;
using Kephas.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Extensions.Validation;

/// <summary>
/// Service providing en enumeration of validators for the validated type using the provided <see cref="IServiceProvider"/>.
/// </summary>
[ScopedService]
public interface IValidatorsProvider
{
    /// <summary>
    /// Gets an enumeration of validators.
    /// </summary>
    /// <param name="validatedType">The type of the validated instance.</param>
    /// <returns>An enumeration of validators.</returns>
    IEnumerable<IValidator> GetValidators(Type validatedType);
}
