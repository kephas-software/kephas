// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceMetadataValueAttributeBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Base class for service metadata value attributes.
/// </summary>
/// <param name="value">The metadata value.</param>
/// <typeparam name="TValue">The metadata value type.</typeparam>
public abstract class ServiceMetadataValueAttributeBase<TValue>(TValue value) : Attribute, IServiceMetadataValue<TValue>
{
    /// <summary>
    /// Gets the metadata value.
    /// </summary>
    /// <value>
    /// The metadata value.
    /// </value>
    public TValue Value { get; } = value;
}