// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceNameAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service name attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Attribute for providing a key to a service implementation.
/// </summary>
/// <param name="value">The service key.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ServiceKeyAttribute(string value) : Attribute, IServiceMetadataValue<string>
{
    /// <summary>Gets the metadata value.</summary>
    /// <value>The metadata value.</value>
    public string Value { get; } = value ?? throw new ArgumentNullException(nameof(value));
}