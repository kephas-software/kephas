// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaTypeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services.Metadata;

namespace Kephas.Serialization;

/// <summary>
/// Service metadata for <see cref="ISerializer"/> services.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MediaTypeAttribute : Attribute, IServiceMetadataValue<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaTypeAttribute"/> class.
    /// </summary>
    /// <param name="value">The media type.</param>
    public MediaTypeAttribute(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the metadata value.
    /// </summary>
    /// <value>
    /// The metadata value.
    /// </value>
    public string Value { get; }
}
