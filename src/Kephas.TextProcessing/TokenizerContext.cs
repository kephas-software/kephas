// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenizerContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the tokenizer context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing;

using System;

using Kephas.Injection;
using Kephas.Services;

/// <summary>
/// A tokenizer context.
/// </summary>
public class TokenizerContext : Context, ITokenizerContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizerContext"/> class.
    /// </summary>
    /// <param name="injector">The injector.</param>
    public TokenizerContext(IInjector injector)
        : base(injector)
    {
    }

    /// <summary>
    /// Gets or sets the transformation.
    /// </summary>
    /// <value>
    /// A function delegate that yields a string.
    /// </value>
    public Func<string, string>? Transformation { get; set; }
}