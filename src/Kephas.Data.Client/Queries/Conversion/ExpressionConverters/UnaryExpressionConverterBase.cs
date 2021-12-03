// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnaryExpressionConverterBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the unary expression converter base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Data.Client.Resources;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Base class for unary expression converters.
    /// </summary>
    public abstract class UnaryExpressionConverterBase : IExpressionConverter
    {
        /// <summary>
        /// The binary expression factory.
        /// </summary>
        private readonly Func<Expression, UnaryExpression> unaryExpressionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryExpressionConverterBase"/> class.
        /// </summary>
        /// <param name="unaryExpressionFactory">The unary expression factory.</param>
        protected UnaryExpressionConverterBase(Func<Expression, UnaryExpression> unaryExpressionFactory)
        {
            unaryExpressionFactory = unaryExpressionFactory ?? throw new System.ArgumentNullException(nameof(unaryExpressionFactory));

            this.unaryExpressionFactory = unaryExpressionFactory;
        }

        /// <summary>
        /// Converts the provided expression to a LINQ expression.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="clientItemType">The client item type.</param>
        /// <param name="lambdaArg">The lambda argument.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        public virtual Expression ConvertExpression(IList<Expression> args, Type clientItemType,
            ParameterExpression lambdaArg)
        {
            if (args.Count != 1)
            {
                throw new DataException(string.Format(Strings.ExpressionConverter_BadArgumentsCount_Exception, args.Count, 1));
            }

            return this.unaryExpressionFactory(args[0]);
        }
    }
}