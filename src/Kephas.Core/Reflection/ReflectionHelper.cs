// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Runtime;

    /// <summary>
    /// Helper class for reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// The empty type infos.
        /// </summary>
        internal static readonly IReadOnlyList<ITypeInfo> EmptyTypeInfos;

        /// <summary>
        /// Initializes static members of the <see cref="ReflectionHelper"/> class.
        /// </summary>
        static ReflectionHelper()
        {
            EmptyTypeInfos = new ReadOnlyCollection<ITypeInfo>(new List<ITypeInfo>());
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
            if (!(memberExpression?.Member is PropertyInfo))
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
            Requires.NotNull(type, nameof(type));

            return string.Concat(type.FullName, ", ", IntrospectionExtensions.GetTypeInfo(type).Assembly.GetName().Name);
        }

        /// <summary>
        /// Gets the full name of the non generic type with the same base name as the provided type.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>The full name of the non generic type.</returns>
        public static string GetNonGenericFullName(this TypeInfo typeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

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
            Requires.NotNull(type, nameof(type));

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
            Requires.NotNull(methodInfo, nameof(methodInfo));

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
            return IsSystemAssembly(assembly.GetName());
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
            var assemblyFullName = assemblyName.FullName;
            return assemblyFullName.StartsWith("System") || assemblyFullName.StartsWith("mscorlib") ||
                   assemblyFullName.StartsWith("Microsoft") || assemblyFullName.StartsWith("vshost32") ||
                   assemblyFullName.StartsWith("Mono");
        }

        /// <summary>
        /// Gets the location for the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly to be checked.</param>
        /// <returns>
        /// The assembly location.
        /// </returns>
        public static string GetLocation(this Assembly assembly)
        {
            if (assembly == null)
            {
                return null;
            }

            var codebaseUri = new Uri(assembly.CodeBase);
            var location = Path.GetDirectoryName(Uri.UnescapeDataString(codebaseUri.AbsolutePath));
            return location;
        }

        /// <summary>
        /// Gets the <see cref="IRuntimeAssemblyInfo"/> for the provided <see cref="Assembly"/> instance.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The provided <see cref="Assembly"/>'s associated <see cref="IRuntimeAssemblyInfo"/>.
        /// </returns>
        public static IRuntimeAssemblyInfo AsRuntimeAssemblyInfo(this Assembly assembly)
        {
            Requires.NotNull(assembly, nameof(assembly));

            return RuntimeAssemblyInfo.GetRuntimeAssembly(assembly);
        }

        /// <summary>
        /// Invokes the <paramref name="methodInfo"/> with the provided parameters,
        /// ensuring in case of an exception that the original exception is thrown.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="arguments">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// The invocation result.
        /// </returns>
        public static object Call(this MethodInfo methodInfo, object instance, params object[] arguments)
        {
            try
            {
                var result = methodInfo.Invoke(instance, arguments);
                return result;
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Gets the most specific type information out of the provided instance.
        /// If the object implements <see cref="IInstance"/>, then it returns
        /// the <see cref="ITypeInfo"/> provided by it, otherwise it returns the <see cref="IRuntimeTypeInfo"/>
        /// of its runtime type.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A type information for the provided object.</returns>
        public static ITypeInfo GetTypeInfo(this object obj)
        {
            var typeInfo = (obj as IInstance)?.GetTypeInfo();
            return typeInfo ?? obj?.GetType().AsRuntimeTypeInfo();
        }
    }
}