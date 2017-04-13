// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GtExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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