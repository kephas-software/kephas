﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GteExpressionConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the gte expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Expression converter for the greater-than-or-equal operator.
    /// </summary>
    [Operator("$gte")]
    public class GteExpressionConverter : BinaryExpressionConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GteExpressionConverter"/> class.
        /// </summary>
        public GteExpressionConverter()
            : base((Func<Expression, Expression, BinaryExpression>)Expression.GreaterThanOrEqual)
        {
        }
    }
}