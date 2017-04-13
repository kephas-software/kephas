// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GteExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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