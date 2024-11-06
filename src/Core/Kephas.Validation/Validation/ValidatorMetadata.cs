// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentValidation;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Validation;

/// <summary>
/// Metadata for <see cref="IValidator{T}"/>.
/// </summary>
public record ValidatorMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatorMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public ValidatorMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        ValidatedType = (Type?)TryGetValue(metadata, "TType");
    }

    /// <summary>
    /// Gets the notification type.
    /// </summary>
    public Type? ValidatedType { get; }
}