﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the language attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.AttributedModel;

using System;

using Kephas.Services;

/// <summary>
/// Indicates the supported language.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class LanguageAttribute : Attribute, IMetadataValue<string[]>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public LanguageAttribute(string value)
    {
        value = value ?? throw new ArgumentNullException(nameof(value));
        this.Value = new[] { value };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public LanguageAttribute(params string[] value)
    {
        this.Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets the metadata value.</summary>
    /// <value>The metadata value.</value>
    public string[] Value { get; }
}