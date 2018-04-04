// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NeqExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the neq expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System.Linq.Expressions;

    /// <summary>
    /// Expression converter for the not-equals operator.
    /// </summary>
    [Operator("$neq")]
    public class NeqExpressionConverter : BinaryExpressionConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NeqExpressionConverter"/> class.
        /// </summary>
        public NeqExpressionConverter()
            : base(Expression.NotEqual)
        {
        }
    }
}