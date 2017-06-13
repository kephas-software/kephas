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
            if (args[0].Type == args[1].Type)
            {
                return args;
            }

            if (args[0].NodeType == ExpressionType.MemberAccess)
            {
                if (args[1].NodeType == ExpressionType.Constant)
                {
                    var value = ((ConstantExpression)args[1]).Value;
                    if (!ReferenceEquals(value, null))
                    {
                        var convertedValue = Convert.ChangeType(value, args[0].Type);
                        return new List<Expression> { args[0], Expression.Constant(convertedValue) };
                    }
                }
            }

            return args;
        }
    }
}