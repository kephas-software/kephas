// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LteExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lte expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Expression converter for the less-than-or-equal operator.
    /// </summary>
    [Operator("$lte")]
    public class LteExpressionConverter : BinaryExpressionConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LteExpressionConverter"/> class.
        /// </summary>
        public LteExpressionConverter()
            : base((Func<Expression, Expression, BinaryExpression>)Expression.GreaterThanOrEqual)
        {
        }
    }
}