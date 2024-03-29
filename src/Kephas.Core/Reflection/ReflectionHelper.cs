﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Helper class for reflection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Helper class for reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        private static readonly Func<AssemblyName, bool> IsSystemAssemblyFuncValue = assemblyName =>
            {
                var assemblyFullName = assemblyName.FullName;
                return assemblyFullName.StartsWith("System")
                    || assemblyFullName.StartsWith("mscorlib")
                    || assemblyFullName.StartsWith("Microsoft")
                    || assemblyFullName.StartsWith("netstandard")
                    || assemblyFullName.StartsWith("vshost32")
                    || assemblyFullName.StartsWith("Mono");
            };

        private static Func<AssemblyName, bool>? isSystemAssemblyFunc = IsSystemAssemblyFuncValue;

        /// <summary>
        /// Sets the callback invoked when <see cref="IsSystemAssembly(System.Reflection.Assembly)"/> is called.
        /// </summary>
        /// <param name="callback">The callback. If <c>null</c>, no check is performed and all assemblies are considered as being not system.</param>
        public static void OnIsSystemAssembly(Func<AssemblyName, bool>? callback)
        {
            isSystemAssemblyFunc = callback;
        }

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
            MemberExpression? memberExpression;

            // if the return value had to be cast to object, the body will be an UnaryExpression
            if (expression.Body is UnaryExpression unary)
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
            if (memberExpression?.Member is not PropertyInfo)
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
            MemberExpression? memberExpression;

            // if the return value had to be cast to object, the body will be an UnaryExpression
            if (expression.Body is UnaryExpression unary)
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
        /// Gets the method indicated by the given expression.
        /// The given expression must be a lambda expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>A <see cref="MethodInfo"/>.</returns>
        public static MethodInfo GetMethodOf(Expression expression)
        {
            return ((MethodCallExpression)((LambdaExpression)expression).Body).Method;
        }

        /// <summary>
        /// Gets the generic method indicated by the given expression.
        /// The given expression must be a lambda expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>A <see cref="MethodInfo"/>.</returns>
        public static MethodInfo GetGenericMethodOf(Expression expression)
        {
            return ((MethodCallExpression)((LambdaExpression)expression).Body).Method.GetGenericMethodDefinition();
        }

        /// <summary>
        /// Gets the method indicated by the given expression.
        /// The given expression must be a lambda expression.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>A <see cref="MethodInfo"/>.</returns>
        public static MethodInfo GetMethodOf<TReturn>(Expression<Func<object, TReturn>> expression)
        {
            return GetMethodOf((Expression)expression);
        }

        /// <summary>
        /// Gets the generic method indicated by the given expression.
        /// The given expression must be a lambda expression.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>A <see cref="MethodInfo"/>.</returns>
        public static MethodInfo GetGenericMethodOf<TReturn>(Expression<Func<object, TReturn>> expression)
        {
            return GetGenericMethodOf((Expression)expression);
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
            type = type ?? throw new ArgumentNullException(nameof(type));

            return string.Concat(type.FullName, ", ", IntrospectionExtensions.GetTypeInfo(type).Assembly.GetName().Name);
        }

        /// <summary>
        /// Gets the full name of the non generic type with the same base name as the provided type.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>The full name of the non generic type.</returns>
        public static string GetNonGenericFullName(this TypeInfo typeInfo)
        {
            typeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));

            var fullName = typeInfo.FullName!;
            if (!typeInfo.IsGenericType)
            {
                return fullName;
            }

            fullName = fullName[..fullName.IndexOf('`')];
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
            type = type ?? throw new ArgumentNullException(nameof(type));

            return GetNonGenericFullName(IntrospectionExtensions.GetTypeInfo(type));
        }

        /// <summary>
        /// Creates the static delegate for the provided static method.
        /// </summary>
        /// <typeparam name="T">The delegate type.</typeparam>
        /// <param name="methodInfo">The method information.</param>
        /// <returns>A static delegate for the provided static method.</returns>
        public static T CreateStaticDelegate<T>(this MethodInfo methodInfo)
        {
            methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));

            return (T)(object)methodInfo.CreateDelegate(typeof(T));
        }

        /// <summary>
        /// Gets a value indicating whether the provided assembly is a system assembly.
        /// </summary>
        /// <param name="assembly">The assembly to be checked.</param>
        /// <returns>
        /// <c>true</c> if the assembly is a system assembly, <c>false</c> if not.
        /// </returns>
        public static bool IsSystemAssembly(this Assembly assembly)
        {
            return isSystemAssemblyFunc?.Invoke(assembly.GetName()) ?? false;
        }

        /// <summary>
        /// Gets a value indicating whether the provided assembly is a system assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly to be checked.</param>
        /// <returns>
        /// <c>true</c> if the assembly is a system assembly, <c>false</c> if not.
        /// </returns>
        public static bool IsSystemAssembly(this AssemblyName assemblyName)
        {
            return isSystemAssemblyFunc?.Invoke(assemblyName) ?? false;
        }

        /// <summary>
        /// Gets the location directory for the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly to be checked.</param>
        /// <returns>
        /// The assembly location directory.
        /// </returns>
        public static string GetLocationDirectory(this Assembly assembly)
        {
            assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

            var location = Path.GetDirectoryName(assembly.Location)!;
            return location;
        }

        /// <summary>
        /// Gets the type's proper properties: public, non-static, and without parameters.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An enumeration of property infos.</returns>
        public static IEnumerable<PropertyInfo> GetTypeProperties(Type type)
        {
            return type.GetRuntimeProperties()
                .Where(p => p.GetMethod != null && !p.GetMethod.IsStatic && p.GetMethod.IsPublic
                            && p.GetIndexParameters().Length == 0);
        }
    }
}