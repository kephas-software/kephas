﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartsWithExpressionConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the starts with expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    /// <summary>
    /// Expression converter for the string.StartsWith operator.
    /// </summary>
    [Operator("$startswith")]
    public class StartsWithExpressionConverter : MethodCallExpressionConverterBase<string, string, bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartsWithExpressionConverter"/> class.
        /// </summary>
        public StartsWithExpressionConverter()
            : base((s1, s2) => s1.StartsWith(s2))
        {
        }
    }
}