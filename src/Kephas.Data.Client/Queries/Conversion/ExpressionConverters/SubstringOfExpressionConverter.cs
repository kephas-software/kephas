﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstringOfExpressionConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the substring of expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    /// <summary>
    /// Expression converter for the string.Contains operator.
    /// </summary>
    [Operator("$substringof")]
    public class SubstringOfExpressionConverter : MethodCallExpressionConverterBase<string, string, bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubstringOfExpressionConverter"/> class.
        /// </summary>
        public SubstringOfExpressionConverter()
            : base((s1, s2) => s1.Contains(s2))
        {
        }
    }
}