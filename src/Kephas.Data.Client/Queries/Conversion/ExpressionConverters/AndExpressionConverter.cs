// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AndExpressionConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the and expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Expression converter for the AND operator.
    /// </summary>
    [Operator("$and")]
    public class AndExpressionConverter : IExpressionConverter
    {
        /// <summary>
        /// Converts the provided expression to a LINQ expression.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        public Expression ConvertExpression(IList<Expression> args)
        {
            return args.Count == 0 ? null : this.JoinExpressions(args, 0);
        }

        /// <summary>
        /// Joins the expressions.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <param name="index">Zero-based index of the expression to be joined.</param>
        /// <returns>
        /// An Expression.
        /// </returns>
        private Expression JoinExpressions(IList<Expression> expressions, int index)
        {
            if (index == expressions.Count - 1)
            {
                return expressions[expressions.Count - 1];
            }

            return Expression.AndAlso(expressions[index], this.JoinExpressions(expressions, index + 1));
        }
    }
}