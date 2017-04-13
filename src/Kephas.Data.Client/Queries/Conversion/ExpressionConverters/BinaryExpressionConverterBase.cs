// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryExpressionConverterBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the binary expression converter base class.
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
    /// Base class for binary expression converters.
    /// </summary>
    public abstract class BinaryExpressionConverterBase : IExpressionConverter
    {
        /// <summary>
        /// The binary expression factory.
        /// </summary>
        private readonly Func<Expression, Expression, BinaryExpression> binaryExpressionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryExpressionConverterBase"/> class.
        /// </summary>
        /// <param name="binaryExpressionFactory">The binary expression factory.</param>
        protected BinaryExpressionConverterBase(Func<Expression, Expression, BinaryExpression> binaryExpressionFactory)
        {
            Requires.NotNull(binaryExpressionFactory, nameof(binaryExpressionFactory));

            this.binaryExpressionFactory = binaryExpressionFactory;
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
            if (args.Count != 2)
            {
                throw new DataException(string.Format(Strings.ExpressionConverter_BadArgumentsCount_Exception, args.Count, 2));
            }

            return this.binaryExpressionFactory(args[0], args[1]);
        }
    }
}