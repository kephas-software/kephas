// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstituteExpressionExpressionVisitor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the SubstituteExpressionExpressionVisitor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq.Expressions
{
    using System.Linq.Expressions;

    /// <summary>
    /// Visitor that replaces an expression with another one.
    /// The rest of the node will stay the same.
    /// </summary>
    public class SubstituteExpressionExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// The search expression.
        /// </summary>
        private readonly Expression searchExpression;

        /// <summary>
        /// The substitute expression.
        /// </summary>
        private readonly Expression substitute;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubstituteExpressionExpressionVisitor" /> class.
        /// </summary>
        /// <param name="searchExpression">The searched expression.</param>
        /// <param name="substitute">The substitute expression.</param>
        public SubstituteExpressionExpressionVisitor(Expression searchExpression, Expression substitute)
        {
            this.searchExpression = searchExpression;
            this.substitute = substitute;
        }

        /// <summary>
        /// Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        public override Expression Visit(Expression node)
        {
            if (node == this.searchExpression)
            {
                return this.substitute;
            }

            return base.Visit(node);
        }
    }
}