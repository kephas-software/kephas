// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionHelper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the expression helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters.Internal
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// An expression helper.
    /// </summary>
    internal static class ExpressionHelper
    {
        /// <summary>
        /// Gets the method information for the provided function definition.
        /// </summary>
        /// <typeparam name="T1">Generic type parameter T1.</typeparam>
        /// <typeparam name="T2">Generic type parameter T2.</typeparam>
        /// <typeparam name="T3">Generic type parameter T3.</typeparam>
        /// <param name="expression">A Func{T1,T2,T3} to process.</param>
        /// <returns>
        /// The method information.
        /// </returns>
        public static MethodInfo GetMethodInfo<T1, T2, T3>(Expression<Func<T1, T2, T3>> expression)
        {
            Requires.NotNull(expression, nameof(expression));

            return ((MethodCallExpression)expression.Body).Method;
        }

        /// <summary>
        /// Gets the method information for the provided function definition.
        /// </summary>
        /// <typeparam name="T1">Generic type parameter T1.</typeparam>
        /// <typeparam name="T2">Generic type parameter T2.</typeparam>
        /// <param name="expression">A Func{T1,T2} to process.</param>
        /// <returns>
        /// The method information.
        /// </returns>
        public static MethodInfo GetMethodInfo<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            Requires.NotNull(expression, nameof(expression));

            return ((MethodCallExpression)expression.Body).Method;
        }

        /// <summary>
        /// Gets the method information for the provided function definition.
        /// </summary>
        /// <typeparam name="T1">Generic type parameter T1.</typeparam>
        /// <param name="expression">A Func{T1} to process.</param>
        /// <returns>
        /// The method information.
        /// </returns>
        public static MethodInfo GetMethodInfo<T1>(Expression<Func<T1>> expression)
        {
            Requires.NotNull(expression, nameof(expression));

            return ((MethodCallExpression)expression.Body).Method;
        }
    }
}