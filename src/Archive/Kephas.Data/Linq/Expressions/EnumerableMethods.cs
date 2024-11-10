// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableMethods.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enumerable methods class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Helper class for accessing the methods of the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class EnumerableMethods
    {
        /// <summary>
        /// Initializes static members of the <see cref="EnumerableMethods"/> class.
        /// </summary>
        static EnumerableMethods()
        {
            EnumerableCountGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IEnumerable<int>)null).Count());
            EnumerableNonEmptyCountGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IEnumerable<int>)null).Count((Func<int, bool>)null));
            EnumerableLongCountGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IEnumerable<int>)null).LongCount());
            EnumerableNonEmptyLongCountGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IEnumerable<int>)null).LongCount((Func<int, bool>)null));
            EnumerableEmptyAnyGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IEnumerable<int>)null).Any());
            EnumerableNonEmptyAnyGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IEnumerable<int>)null).Any(null));
            EnumerableAllGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IEnumerable<int>)null).All(null));
            EnumerableOfType = ReflectionHelper.GetGenericMethodOf(_ => ((System.Collections.IEnumerable)null).OfType<int>());
            EnumerableSelectGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IEnumerable<int>)null).Select(i => i));
            EnumerableTakeGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IEnumerable<int>)null).Take(0));
            EnumerableContainsGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IEnumerable<int>)null).Contains(0));
        }

        /// <summary>
        /// Gets the enumerable count non empty generic.
        /// </summary>
        /// <value>
        /// The enumerable count non empty generic.
        /// </value>
        public static MethodInfo EnumerableNonEmptyCountGeneric { get; }

        /// <summary>
        /// Gets the enumerable long count non empty generic.
        /// </summary>
        /// <value>
        /// The enumerable long count non empty generic.
        /// </value>
        public static MethodInfo EnumerableNonEmptyLongCountGeneric { get; }

        /// <summary>
        /// Gets the enumerable count generic.
        /// </summary>
        /// <value>
        /// The enumerable count generic.
        /// </value>
        public static MethodInfo EnumerableCountGeneric { get; }

        /// <summary>
        /// Gets the enumerable long count generic.
        /// </summary>
        /// <value>
        /// The enumerable long count generic.
        /// </value>
        public static MethodInfo EnumerableLongCountGeneric { get; }

        /// <summary>
        /// Gets the enumerable all generic.
        /// </summary>
        public static MethodInfo EnumerableAllGeneric { get; }

        /// <summary>
        /// Gets the enumerable empty any generic.
        /// </summary>
        public static MethodInfo EnumerableEmptyAnyGeneric { get; }

        /// <summary>
        /// Gets the enumerable non empty any generic.
        /// </summary>
        public static MethodInfo EnumerableNonEmptyAnyGeneric { get; }

        /// <summary>
        /// Gets the type of the enumerable of.
        /// </summary>
        public static MethodInfo EnumerableOfType { get; }

        /// <summary>
        /// Gets the enumerable select generic.
        /// </summary>
        public static MethodInfo EnumerableSelectGeneric { get; }

        /// <summary>
        /// Gets the enumerable take generic.
        /// </summary>
        public static MethodInfo EnumerableTakeGeneric { get; }

        /// <summary>
        /// Gets the enumerable contains generic.
        /// </summary>
        /// <value>
        /// The enumerable contains generic.
        /// </value>
        public static MethodInfo EnumerableContainsGeneric { get; }
    }
}