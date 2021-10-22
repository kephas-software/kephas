// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enumerable extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Extension methods for all kinds of (typed) enumerable data (Array, List, ...)
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        ///   Performs an action for each item in the enumerable.
        /// </summary>
        /// <typeparam name = "T">The enumerable data type.</typeparam>
        /// <param name = "values">The data values.</param>
        /// <param name = "action">The action to be performed.</param>
        /// <example>
        /// <code>
        ///   var values = new[] { "1", "2", "3" };
        ///   values.ConvertList&lt;string, int&gt;().ForEach(Console.WriteLine);
        /// </code>
        /// </example>
        /// <remarks>
        ///   This method was intended to return the passed values to provide method chaining. However due to deferred execution the compiler would actually never run the entire code at all.
        /// </remarks>
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
            {
                action(value);
            }
        }
    }
}