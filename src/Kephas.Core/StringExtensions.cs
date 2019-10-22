// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the string extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Splits the string using a controller function.
        /// </summary>
        /// <param name="str">The string to act on.</param>
        /// <param name="isSeparator">The function indicating whether a character is separator.</param>
        /// <param name="removeEmptyEntries">Optional. True to remove empty entries.</param>
        /// <returns>
        /// An enumeration of string splits.
        /// </returns>
        public static IEnumerable<string> Split(this string str, Func<char, bool> isSeparator, bool removeEmptyEntries = true)
        {
            Requires.NotNull(isSeparator, nameof(isSeparator));

            if (string.IsNullOrEmpty(str))
            {
                yield break;
            }

            var nextPiece = 0;

            for (var c = 0; c < str.Length; c++)
            {
                if (isSeparator(str[c]))
                {
                    var chunkLength = c - nextPiece;
                    if (chunkLength > 0 || !removeEmptyEntries)
                    {
                        yield return str.Substring(nextPiece, chunkLength);
                    }

                    nextPiece = c + 1;
                }
            }

            if (nextPiece < str.Length || !removeEmptyEntries)
            {
                yield return str.Substring(nextPiece);
            }
        }

        /// <summary>
        /// Splits the string using a separator and quoting characters.
        /// </summary>
        /// <param name="str">The string to act on.</param>
        /// <param name="separator">The separator characters.</param>
        /// <param name="quote">The quote characters.</param>
        /// <param name="removeEmptyEntries">Optional. True to remove empty entries.</param>
        /// <returns>
        /// An enumeration of string splits.
        /// </returns>
        public static IEnumerable<string> Split(
            this string str,
            char[] separator,
            char[] quote,
            bool removeEmptyEntries = true)
        {
            Requires.NotNullOrEmpty(separator, nameof(separator));
            Requires.NotNull(quote, nameof(quote));

            char? quoteChar = null;
            return str.Split(
                c =>
                    {
                        if (quoteChar == null)
                        {
                            if (quote.Contains(c))
                            {
                                quoteChar = c;
                                return false;
                            }

                            return separator.Contains(c);
                        }

                        if (c == quoteChar)
                        {
                            quoteChar = null;
                        }

                        return false;
                    },
                removeEmptyEntries);
        }

        /// <summary>
        /// Formats the provided string with the indicated arguments.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// The formatted string.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, params object[] args)
            => string.Format(format, args);

        /// <summary>
        /// Concatenates the elements of an object array, using the specified separator between
        /// each element.
        /// </summary>
        /// <param name="separator">
        /// The string to use as a separator. <paramref name="separator"/> is included in the returned string
        /// only if values has more than one element.
        /// </param>
        /// <param name="args">An array that contains the elements to concatenate.</param>
        /// <returns>
        /// A string.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string JoinWith(this string separator, params object[] args)
            => string.Join(separator, args);
    }
}