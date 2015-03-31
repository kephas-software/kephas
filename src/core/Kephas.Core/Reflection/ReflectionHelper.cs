// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Helper class for reflection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Helper class for reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Retrieves the property name from a lambda expression.
        /// </summary>
        /// <typeparam name="T">The type from which the property name is extracted.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <returns>The property name.</returns>
        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            return GetPropertyName<T, object>(expression);
        }

        /// <summary>
        /// Retrieves the property name from a lambda expression.
        /// </summary>
        /// <typeparam name="T">The type from which the property name is extracted.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <returns>
        /// The property name.
        /// </returns>
        /// <exception cref="System.ArgumentException">Expected property expression.</exception>
        public static string GetPropertyName<T, TValue>(Expression<Func<T, TValue>> expression)
        {
            MemberExpression memberExpression;

            // if the return value had to be cast to object, the body will be an UnaryExpression
            var unary = expression.Body as UnaryExpression;
            if (unary != null)
            {
                // the operand is the "real" property access
                memberExpression = unary.Operand as MemberExpression;
            }
            else
            {
                // in case the property is of type object the body itself is the correct expression
                memberExpression = expression.Body as MemberExpression;
            }

            // as before:
            if (memberExpression == null || !(memberExpression.Member is PropertyInfo))
            {
                throw new ArgumentException("Expected property expression.");
            }

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Retrieves the property name from a lambda expression.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <returns>The property name.</returns>
        public static string GetStaticPropertyName(Expression<Func<object>> expression)
        {
            MemberExpression memberExpression;

            // if the return value had to be cast to object, the body will be an UnaryExpression
            var unary = expression.Body as UnaryExpression;
            if (unary != null)
            {
                // the operand is the "real" property access
                memberExpression = unary.Operand as MemberExpression;
            }
            else
            {
                // in case the property is of type object the body itself is the correct expression
                memberExpression = expression.Body as MemberExpression;
            }

            // as before:
            if (memberExpression == null || !(memberExpression.Member is PropertyInfo))
            {
                throw new ArgumentException("Expected property expression");
            }

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Gets the assembly qualified name without the version and public key information.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The assembly qualified name without the version and public key information.
        /// </returns>
        public static string GetAssemblyQualifiedShortName(this Type type)
        {
            Contract.Requires(type != null);

            return string.Concat(type.FullName, ", ", type.GetTypeInfo().Assembly.GetName().Name);
        }

        /// <summary>
        /// Gets the full name of the non generic type with the same base name as the provided type.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>The full name of the non generic type.</returns>
        public static string GetNonGenericFullName(this TypeInfo typeInfo)
        {
            Contract.Requires(typeInfo != null);

            var fullName = typeInfo.FullName;
            if (!typeInfo.IsGenericType)
            {
                return fullName;
            }

            fullName = fullName.Substring(0, fullName.IndexOf('`'));
            return fullName;
        }

        /// <summary>
        /// Gets the full name of the non generic type with the same base name as the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The full name of the non generic type.
        /// </returns>
        public static string GetNonGenericFullName(this Type type)
        {
            Contract.Requires(type != null);

            return GetNonGenericFullName(type.GetTypeInfo());
        }

        /// <summary>
        /// Creates the static delegate for the provided static method.
        /// </summary>
        /// <typeparam name="T">The delegate type.</typeparam>
        /// <param name="methodInfo">The method information.</param>
        /// <returns>A static delegate for the provided static method.</returns>
        public static T CreateStaticDelegate<T>(this MethodInfo methodInfo)
        {
            Contract.Requires(methodInfo != null);

            return (T)(object)methodInfo.CreateDelegate(typeof(T));
        }
    }
}