// --------------------------------------------------------------------------------------------------------------------
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
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Helper class for reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// The lower case offset.
        /// </summary>
        private const int LowerCaseOffset = 'a' - 'A';

        private static readonly Func<AssemblyName, bool> IsSystemAssemblyFuncValue = assemblyName =>
            {
                var assemblyFullName = assemblyName.FullName;
                return assemblyFullName.StartsWith("System")
                    || assemblyFullName.StartsWith("mscorlib")
                    || assemblyFullName.StartsWith("Microsoft")
                    || assemblyFullName.StartsWith("vshost32")
                    || assemblyFullName.StartsWith("Mono");
            };

        /// <summary>
        /// Gets or sets the function to check whether an assembly is a system assembly.
        /// </summary>
        /// <value>
        /// A function delegate that yields a bool.
        /// </value>
        public static Func<AssemblyName, bool> IsSystemAssemblyFunc { get; set; } = IsSystemAssemblyFuncValue;

        /// <summary>
        /// Converts the provided string value to camel case.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>
        /// The camel case representation of the provided value.
        /// </returns>
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var len = value.Length;
            var newValue = new char[len];
            var firstPart = true;
            var isAllUpper = true;

            for (var i = 0; i < len; ++i)
            {
                var c0 = value[i];
                var c1 = i < len - 1 ? value[i + 1] : 'A';
                var c0isUpper = c0 >= 'A' && c0 <= 'Z';
                var c1isUpper = c1 >= 'A' && c1 <= 'Z';

                if (firstPart && c0isUpper && (c1isUpper || i == 0))
                {
                    c0 = (char)(c0 + LowerCaseOffset);
                }
                else
                {
                    firstPart = false;
                }

                if (!c0isUpper && c0 != '-' && c0 != '_')
                {
                    isAllUpper = false;
                }

                newValue[i] = c0;
            }

            return isAllUpper && len > 1 ? value : new string(newValue);
        }

        /// <summary>
        /// Converts the provided string value to Pascal case.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>
        /// The Pascal case representation of the provided value.
        /// </returns>
        public static string ToPascalCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (value.IndexOf('_') >= 0 || value.IndexOf('-') >= 0)
            {
                if (value.IndexOf("__") >= 0 || value.IndexOf("--") >= 0)
                {
                    return value;
                }

                var parts = value.Split('_', '-');
                var sb = StringBuilderThreadStatic.Allocate();
                foreach (var part in parts)
                {
                    var str = part.ToCamelCase();
                    sb.Append(char.ToUpper(str[0]) + str.SafeSubstring(1, str.Length));
                }

                return StringBuilderThreadStatic.ReturnAndFree(sb);
            }

            var camelCase = value.ToCamelCase();
            return char.ToUpper(camelCase[0]) + camelCase.SafeSubstring(1, camelCase.Length);
        }

        /// <summary>
        /// A string extension method that creates a safe substring from the given index.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>
        /// A string.
        /// </returns>
        public static string SafeSubstring(this string value, int startIndex)
        {
            return SafeSubstring(value, startIndex, value.Length);
        }

        /// <summary>
        /// A string extension method that safe substring from the given index with the given length.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        /// A string.
        /// </returns>
        public static string SafeSubstring(this string value, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (value.Length >= (startIndex + length))
            {
                return value.Substring(startIndex, length);
            }

            return value.Length > startIndex ? value.Substring(startIndex) : string.Empty;
        }

        /// <summary>
        /// Indicates whether the identifier is private.
        /// </summary>
        /// <param name="identifier">The identifier to act on.</param>
        /// <returns>
        /// True if the identifier is private, false if not.
        /// </returns>
        public static bool IsPrivate(this string identifier)
        {
            return identifier.StartsWith("_") || identifier.StartsWith("#");
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
            MemberExpression? memberExpression;

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
            return IsSystemAssemblyFunc?.Invoke(assembly.GetName()) ?? false;
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
            return IsSystemAssemblyFunc?.Invoke(assemblyName) ?? false;
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
        /// Invokes the <paramref name="methodInfo"/> with the provided parameters,
        /// ensuring in case of an exception that the original exception is thrown.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="arguments">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// The invocation result.
        /// </returns>
        public static object? Call(this MethodInfo methodInfo, object? instance, params object?[] arguments)
        {
            try
            {
                var result = methodInfo.Invoke(instance, arguments);
                return result;
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException!;
            }
        }

        /// <summary>
        /// Use separate cache internally to avoid reallocations and cache misses.
        /// </summary>
        internal static class StringBuilderThreadStatic
        {
            /// <summary>
            /// The cache.
            /// </summary>
            [ThreadStatic]
            private static StringBuilder? cache;

            /// <summary>
            /// Allocates a new string builder.
            /// </summary>
            /// <returns>
            /// A StringBuilder.
            /// </returns>
            public static StringBuilder Allocate()
            {
                var ret = cache;
                if (ret == null)
                {
                    return new StringBuilder();
                }

                ret.Length = 0;
                cache = null;  // don't re-issue cached instance until it's freed
                return ret;
            }

            /// <summary>
            /// Frees the given string builder.
            /// </summary>
            /// <param name="sb">The string builder.</param>
            public static void Free(StringBuilder sb)
            {
                cache = sb;
            }

            /// <summary>
            /// Returns and frees the given string builder.
            /// </summary>
            /// <param name="sb">The string builder.</param>
            /// <returns>
            /// The string.
            /// </returns>
            public static string ReturnAndFree(StringBuilder sb)
            {
                var ret = sb.ToString();
                cache = sb;
                return ret;
            }
        }
    }
}