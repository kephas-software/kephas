﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionStringExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the reflection string extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// String extensions for reflection purposes.
    /// </summary>
    public static class ReflectionStringExtensions
    {
        /// <summary>
        /// The lower case offset.
        /// </summary>
        private const int LowerCaseOffset = 'a' - 'A';

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
        /// Use separate cache internally to avoid reallocations and cache misses.
        /// </summary>
        internal static class StringBuilderThreadStatic
        {
            /// <summary>
            /// The cache.
            /// </summary>
            [ThreadStatic]
            private static StringBuilder cache;

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