// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LtExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lt expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Expression converter for the less-than operator.
    /// </summary>
    [Operator("$lt")]
    public class LtExpressionConverter : BinaryExpressionConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtExpressionConverter"/> class.
        /// </summary>
        public LtExpressionConverter()
            : base((Func<Expression, Expression, BinaryExpression>)Expression.LessThan)
        {
        }
    }
}