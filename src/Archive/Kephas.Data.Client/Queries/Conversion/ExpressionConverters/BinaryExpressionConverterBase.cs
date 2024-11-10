// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryExpressionConverterBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
            binaryExpressionFactory = binaryExpressionFactory ?? throw new System.ArgumentNullException(nameof(binaryExpressionFactory));

            this.binaryExpressionFactory = binaryExpressionFactory;
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
            if (args.Count != 2)
            {
                throw new DataException(string.Format(Strings.ExpressionConverter_BadArgumentsCount_Exception, args.Count, 2));
            }

            args = this.PreProcessArguments(args);
            return this.binaryExpressionFactory(args[0], args[1]);
        }

        /// <summary>
        /// Pre processes the arguments to make their types match.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// A list of preprocessed arguments.
        /// </returns>
        protected virtual IList<Expression> PreProcessArguments(IList<Expression> args)
        {
            return ExpressionHelper.NormalizeBinaryExpressionArgs(args);
        }
    }
}