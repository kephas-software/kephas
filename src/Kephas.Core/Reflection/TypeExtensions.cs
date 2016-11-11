// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    using Kephas.Dynamic;

    /// <summary>
    /// Extension methods for types.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the <see cref="IDynamicTypeInfo"/> for the provided <see cref="Type"/> instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The provided <see cref="Type"/>'s associated <see cref="IDynamicTypeInfo"/>.
        /// </returns>
        public static IDynamicTypeInfo AsDynamicTypeInfo(this Type type)
        {
            Contract.Requires(type != null);

            return DynamicTypeInfo.GetDynamicType(type);
        }

        /// <summary>
        /// Gets a value indicating whether the type implements <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the type is enumerable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnumerable(this Type type)
        {
            return type != null && typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Gets a value indicating whether the type implements <see cref="IQueryable"/>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the type is enumerable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsQueryable(this Type type)
        {
            return type != null && typeof(IQueryable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Tries to get the item type of the generic enumerable type.
        /// </summary>
        /// <param name="type">The enumerable type.</param>
        /// <returns>The item type if the provided type is enumerable, otherwise <c>null</c>.</returns>
        public static Type TryGetEnumerableItemType(this Type type)
        {
            return TryGetEnumerableItemType(type, typeof(IEnumerable<>));
        }

        /// <summary>
        /// Tries to get the item type of the generic queryable type.
        /// </summary>
        /// <param name="type">The queryable type.</param>
        /// <returns>The item type if the provided type is queryable, otherwise <c>null</c>.</returns>
        public static Type TryGetQueryableItemType(this Type type)
        {
            return TryGetEnumerableItemType(type, typeof(IQueryable<>));
        }

        /// <summary>
        /// Tries to get the item type of the generic collection type.
        /// </summary>
        /// <param name="type">The collection type.</param>
        /// <returns>The item type if the provided type is a collection, otherwise <c>null</c>.</returns>
        public static Type TryGetCollectionItemType(this Type type)
        {
            return TryGetEnumerableItemType(type, typeof(ICollection<>));
        }

        /// <summary>
        /// Gets the generic item type of the enumerable type.
        /// </summary>
        /// <param name="type">The enumerable type.</param>
        /// <param name="enumerableGenericType">Type of the enumerable generic.</param>
        /// <returns>
        /// The enumerable item type.
        /// </returns>
        public static Type TryGetEnumerableItemType(this Type type, Type enumerableGenericType)
        {
            if (!type.IsEnumerable())
            {
                return null;
            }

            Func<Type, bool> isRequestedEnumerable = t =>
                {
                    var ti = t.GetTypeInfo();
                    return ti.IsGenericType && ti.GetGenericTypeDefinition() == enumerableGenericType;
                };
            var enumerableType = isRequestedEnumerable(type)
                                     ? type
                                     : type.GetTypeInfo().ImplementedInterfaces.SingleOrDefault(isRequestedEnumerable);
            return enumerableType?.GetTypeInfo().GenericTypeArguments[0];
        }

        /// <summary>
        /// Indicates whether the type is a constructed generic of the provided open generic type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="openGenericType">The open generic type.</param>
        /// <returns>
        /// <c>true</c> if the type is a constructed generic of the provided open generic type, <c>false</c> otherwise.
        /// </returns>
        public static bool IsConstructedGenericOf(this Type type, Type openGenericType)
        {
            Contract.Requires(type != null);

            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == openGenericType;
        }
    }
}
