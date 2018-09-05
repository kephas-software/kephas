// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GtExpressionConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the gt expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Expression converter for the greater-than operator.
    /// </summary>
    [Operator("$gt")]
    public class GtExpressionConverter : BinaryExpressionConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GtExpressionConverter"/> class.
        /// </summary>
        public GtExpressionConverter()
            : base((Func<Expression, Expression, BinaryExpression>)Expression.GreaterThan)
        {
        }
    }
}