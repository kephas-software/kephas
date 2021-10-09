// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the expression helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    /// <summary>
    /// An expression helper.
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// Gets the method information for the provided function definition.
        /// </summary>
        /// <typeparam name="T1">Generic type parameter T1.</typeparam>
        /// <typeparam name="T2">Generic type parameter T2.</typeparam>
        /// <typeparam name="T3">Generic type parameter T3.</typeparam>
        /// <typeparam name="T4">Generic type parameter T4.</typeparam>
        /// <typeparam name="T5">Generic type parameter T5.</typeparam>
        /// <param name="expression">A Func{T1,T2,T3,T4,T5} to process.</param>
        /// <returns>
        /// The method information.
        /// </returns>
        public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5>> expression)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            return ((MethodCallExpression)expression.Body).Method;
        }

        /// <summary>
        /// Gets the method information for the provided function definition.
        /// </summary>
        /// <typeparam name="T1">Generic type parameter T1.</typeparam>
        /// <typeparam name="T2">Generic type parameter T2.</typeparam>
        /// <typeparam name="T3">Generic type parameter T3.</typeparam>
        /// <typeparam name="T4">Generic type parameter T4.</typeparam>
        /// <param name="expression">A Func{T1,T2,T3,T4} to process.</param>
        /// <returns>
        /// The method information.
        /// </returns>
        public static MethodInfo GetMethodInfo<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4>> expression)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            return ((MethodCallExpression)expression.Body).Method;
        }

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
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

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
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

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
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            return ((MethodCallExpression)expression.Body).Method;
        }

        /// <summary>
        /// Gets an expression for the value so that it has a compatible type with the member type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>
        /// The converted value expression.
        /// </returns>
        public static Expression GetConvertedValueExpression(object value, Type targetType)
        {
            object convertedValue;
            var nonNullableMemberType = targetType.GetNonNullableType();
            try
            {
                convertedValue = Convert.ChangeType(value, nonNullableMemberType);
            }
            catch
            {
                return Expression.Convert(Expression.Constant(value), targetType);
            }

            var valueExpression = targetType == nonNullableMemberType
                                      ? (Expression)Expression.Constant(convertedValue)
                                      : Expression.Convert(Expression.Constant(convertedValue), targetType);
            return valueExpression;
        }

        /// <summary>
        /// Normalizes the binary expression arguments.
        /// </summary>
        /// <remarks>
        /// In comparisons, the client may send mismatched types, mainly due to untyped languages like JavaScript.
        /// This method converts the second operand to the type of the member represented by the first operand, so
        /// that the comparison does not fail.
        /// </remarks>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// A list of argument expressions.
        /// </returns>
        public static IList<Expression> NormalizeBinaryExpressionArgs(IList<Expression> args)
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
                    if (value != null)
                    {
                        var valueExpression = GetConvertedValueExpression(value, args[0].Type);
                        return new List<Expression> { args[0], valueExpression };
                    }
                }
            }

            return args;
        }
    }
}