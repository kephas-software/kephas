// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the not expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System.Linq.Expressions;

    /// <summary>
    /// Expression converter for the logical NOT operator.
    /// </summary>
    [Operator("$not")]
    public class NotExpressionConverter : UnaryExpressionConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotExpressionConverter"/> class.
        /// </summary>
        public NotExpressionConverter()
            : base(Expression.Not)
        {
        }
    }
}