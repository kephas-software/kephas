﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableMethods.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the queryable methods class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq.Expressions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Helper class for accessing the methods of the <see cref="IQueryable{T}"/> interface.
    /// </summary>
    public static class QueryableMethods
    {
        /// <summary>
        /// Initializes static members of the <see cref="QueryableMethods"/> class.
        /// </summary>
        static QueryableMethods()
        {
            QueryableOrderByGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).OrderBy((Expression<Func<int, int>>)null));
            QueryableOrderByDescendingGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).OrderByDescending((Expression<Func<int, int>>)null));
            QueryableThenByGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IOrderedQueryable<int>)null).ThenBy((Expression<Func<int, int>>)null));
            QueryableThenByDescendingGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IOrderedQueryable<int>)null).ThenByDescending((Expression<Func<int, int>>)null));
            QueryableCountGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Count());
            QueryableNonEmptyCountGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Count((Expression<Func<int, bool>>)null));
            QueryableLongCountGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).LongCount());
            QueryableNonEmptyLongCountGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).LongCount((Expression<Func<int, bool>>)null));
            QueryableSkipGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Skip(0));
            QueryableWhereGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Where((Expression<Func<int, bool>>)null));
            QueryableEmptyAnyGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Any());
            QueryableNonEmptyAnyGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Any(null));
            QueryableAllGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).All(null));
            QueryableMaxGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Max());
            QueryableMaxNonEmptyGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Max((Expression<Func<int, int>>)null));
            QueryableMinGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Min());
            QueryableMinNonEmptyGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Min((Expression<Func<int, int>>)null));
            QueryableOfType = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable)null).OfType<int>());
            QueryableSelectGeneric = ReflectionHelper.GetGenericMethodOf(_ =>
                                                     from i in (IQueryable<int>)null
                                                     select i);
            QueryableTakeGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Take(0));
            QueryableContainsGeneric = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryable<int>)null).Contains(0));
        }

        /// <summary>
        /// Gets the queryable all generic.
        /// </summary>
        public static MethodInfo QueryableAllGeneric { get; }

        /// <summary>
        /// Gets the queryable count generic.
        /// </summary>
        public static MethodInfo QueryableCountGeneric { get; }

        /// <summary>
        /// Gets the queryable long count generic.
        /// </summary>
        public static MethodInfo QueryableLongCountGeneric { get; }

        /// <summary>
        /// Gets the queryable empty any generic.
        /// </summary>
        public static MethodInfo QueryableEmptyAnyGeneric { get; }

        /// <summary>
        /// Gets the queryable non empty any generic.
        /// </summary>
        public static MethodInfo QueryableNonEmptyAnyGeneric { get; }

        /// <summary>
        /// Gets the type of the queryable of.
        /// </summary>
        public static MethodInfo QueryableOfType { get; }

        /// <summary>
        /// Gets the queryable order by descending generic.
        /// </summary>
        public static MethodInfo QueryableOrderByDescendingGeneric { get; }

        /// <summary>
        /// Gets the queryable order by generic.
        /// </summary>
        public static MethodInfo QueryableOrderByGeneric { get; }

        /// <summary>
        /// Gets the queryable select generic.
        /// </summary>
        public static MethodInfo QueryableSelectGeneric { get; }

        /// <summary>
        /// Gets the queryable skip generic.
        /// </summary>
        public static MethodInfo QueryableSkipGeneric { get; }

        /// <summary>
        /// Gets the queryable take generic.
        /// </summary>
        public static MethodInfo QueryableTakeGeneric { get; }

        /// <summary>
        /// Gets the queryable then by descending generic.
        /// </summary>
        public static MethodInfo QueryableThenByDescendingGeneric { get; }

        /// <summary>
        /// Gets the queryable then by generic.
        /// </summary>
        public static MethodInfo QueryableThenByGeneric { get; }

        /// <summary>
        /// Gets the queryable where generic.
        /// </summary>
        public static MethodInfo QueryableWhereGeneric { get; }

        /// <summary>
        /// Gets the queryable count non empty generic.
        /// </summary>
        /// <value>
        /// The queryable count non empty generic.
        /// </value>
        public static MethodInfo QueryableNonEmptyCountGeneric { get; }

        /// <summary>
        /// Gets the queryable long count non empty generic.
        /// </summary>
        /// <value>
        /// The queryable long count non empty generic.
        /// </value>
        public static MethodInfo QueryableNonEmptyLongCountGeneric { get; }

        /// <summary>
        /// Gets the queryable max generic.
        /// </summary>
        /// <value>
        /// The queryable max generic.
        /// </value>
        public static MethodInfo QueryableMaxGeneric { get; }

        /// <summary>
        /// Gets the queryable max non-empty generic.
        /// </summary>
        /// <value>
        /// The queryable max non-empty generic.
        /// </value>
        public static MethodInfo QueryableMaxNonEmptyGeneric { get; }

        /// <summary>
        /// Gets the queryable min generic.
        /// </summary>
        /// <value>
        /// The queryable min generic.
        /// </value>
        public static MethodInfo QueryableMinGeneric { get; }

        /// <summary>
        /// Gets the queryable min non-empty generic.
        /// </summary>
        /// <value>
        /// The queryable min non-empty generic.
        /// </value>
        public static MethodInfo QueryableMinNonEmptyGeneric { get; }

        /// <summary>
        /// Gets the queryable contains generic.
        /// </summary>
        /// <value>
        /// The queryable contains generic.
        /// </value>
        public static MethodInfo QueryableContainsGeneric { get; }
    }
}