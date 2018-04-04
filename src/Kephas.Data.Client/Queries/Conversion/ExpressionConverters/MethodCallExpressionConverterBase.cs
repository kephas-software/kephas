// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodCallExpressionConverterBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the method call expression converter base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Data.Client.Queries.Conversion.ExpressionConverters.Internal;
    using Kephas.Data.Client.Resources;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Base class for MethodCallExpression based converters.
    /// </summary>
    public abstract class MethodCallExpressionConverterBase : IExpressionConverter
    {
        /// <summary>
        /// Information describing the method.
        /// </summary>
        private readonly MethodInfo methodInfo;

        /// <summary>
        /// Number of accepted arguments.
        /// </summary>
        private readonly int argCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallExpressionConverterBase"/> class.
        /// </summary>
        /// <param name="methodInfo">Information describing the method.</param>
        protected MethodCallExpressionConverterBase(MethodInfo methodInfo)
        {
            Requires.NotNull(methodInfo, nameof(methodInfo));

            this.methodInfo = methodInfo;
            this.argCount = methodInfo.GetParameters().Length;
            if (!methodInfo.IsStatic)
            {
                this.argCount++;
            }
        }

        /// <summary>
        /// Converts the provided expression to a LINQ expression.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        public Expression ConvertExpression(IList<Expression> args)
        {
            if (args.Count != this.argCount)
            {
                throw new DataException(string.Format(Strings.ExpressionConverter_BadArgumentsCount_Exception, args.Count, this.argCount));
            }

            return this.methodInfo.IsStatic
                       ? Expression.Call(null, this.methodInfo, args)
                       : Expression.Call(args[0], this.methodInfo, args.Skip(1));
        }
    }

    /// <summary>
    /// Base class for MethodCallExpression based converters for functions taking no arguments.
    /// </summary>
    public abstract class MethodCallExpressionConverterBase<TReturn> : MethodCallExpressionConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallExpressionConverterBase{TReturn}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected MethodCallExpressionConverterBase(Expression<Func<TReturn>> expression)
            : base(ExpressionHelper.GetMethodInfo(expression))
        {
        }
    }

    /// <summary>
    /// Base class for MethodCallExpression based converters for functions taking one argument.
    /// </summary>
    public abstract class MethodCallExpressionConverterBase<TArg, TReturn> : MethodCallExpressionConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallExpressionConverterBase{TArg, TReturn}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected MethodCallExpressionConverterBase(Expression<Func<TArg, TReturn>> expression)
            : base(ExpressionHelper.GetMethodInfo(expression))
        {
        }
    }

    /// <summary>
    /// Base class for MethodCallExpression based converters for functions taking two arguments.
    /// </summary>
    public abstract class MethodCallExpressionConverterBase<TArg1, TArg2, TReturn> : MethodCallExpressionConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallExpressionConverterBase{TArg1, TArg2, TReturn}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected MethodCallExpressionConverterBase(Expression<Func<TArg1, TArg2, TReturn>> expression)
            : base(ExpressionHelper.GetMethodInfo(expression))
        {
        }
    }
}