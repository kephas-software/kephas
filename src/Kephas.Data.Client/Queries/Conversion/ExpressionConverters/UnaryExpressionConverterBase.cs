// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnaryExpressionConverterBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    public class UnaryExpressionConverterBase : IExpressionConverter
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
            Requires.NotNull(unaryExpressionFactory, nameof(unaryExpressionFactory));

            this.unaryExpressionFactory = unaryExpressionFactory;
        }

        /// <summary>
        /// Converts the provided expression to a LINQ expression.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        public virtual Expression ConvertExpression(IList<Expression> args)
        {
            if (args.Count != 1)
            {
                throw new DataException(string.Format(Strings.ExpressionConverter_BadArgumentsCount_Exception, args.Count, 1));
            }

            return this.unaryExpressionFactory(args[0]);
        }
    }
}