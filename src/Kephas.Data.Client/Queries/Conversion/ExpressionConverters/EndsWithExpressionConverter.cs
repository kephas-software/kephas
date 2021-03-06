﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndsWithExpressionConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ends with expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    /// <summary>
    /// Expression converter for the string.EndsWith operator.
    /// </summary>
    [Operator("$endswith")]
    public class EndsWithExpressionConverter : MethodCallExpressionConverterBase<string, string, bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndsWithExpressionConverter"/> class.
        /// </summary>
        public EndsWithExpressionConverter()
            : base((s1, s2) => s1.EndsWith(s2))
        {
        }
    }
}