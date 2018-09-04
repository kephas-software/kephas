// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotExpressionConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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