// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionKindAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity.AttributedModel;

using System;

using Kephas.Injection;

/// <summary>
/// Indicates the supported connection kind.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ConnectionKindAttribute : Attribute, IMetadataValue<string[]>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionKindAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ConnectionKindAttribute(string value)
    {
        value = value ?? throw new ArgumentNullException(nameof(value));
        this.Value = new[] { value };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionKindAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ConnectionKindAttribute(params string[] value)
    {
        this.Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets the metadata value.</summary>
    /// <value>The metadata value.</value>
    public string[] Value { get; }
}