// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the eq expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System.Linq.Expressions;

    /// <summary>
    /// Expression converter for the equals operator.
    /// </summary>
    [Operator("$eq")]
    public class EqExpressionConverter : BinaryExpressionConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EqExpressionConverter"/> class.
        /// </summary>
        public EqExpressionConverter()
            : base(Expression.Equal)
        {
        }
    }
}