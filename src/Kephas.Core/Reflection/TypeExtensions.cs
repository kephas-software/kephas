// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for types.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// The object type.
        /// </summary>
        internal static readonly Type ObjectType = typeof(object);

        /// <summary>
        /// Gets the <see cref="IRuntimeTypeInfo"/> for the provided <see cref="Type"/> instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The provided <see cref="Type"/>'s associated <see cref="IRuntimeTypeInfo"/>.
        /// </returns>
        public static IRuntimeTypeInfo AsRuntimeTypeInfo(this Type type)
        {
            Requires.NotNull(type, nameof(type));

            return RuntimeTypeInfo.GetRuntimeType(type);
        }

        /// <summary>
        /// Gets the type wrapped by the <see cref="Nullable{T}"/> or,
        /// if the type is not a nullable type, the type itself.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>A <see cref="Type"/> instance.</returns>
        public static Type GetNonNullableType(this Type type)
        {
            Requires.NotNull(type, nameof(type));

            return IntrospectionExtensions.GetTypeInfo(type).GetNonNullableType().AsType();
        }

        /// <summary>
        /// Indicates whether the type is an instance of the generic <see cref="Nullable{T}"/> type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(this Type type)
        {
            Requires.NotNull(type, nameof(type));

            return IntrospectionExtensions.GetTypeInfo(type).IsNullableType();
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
            return type != null
                   && IntrospectionExtensions.GetTypeInfo(typeof(IEnumerable))
                       .IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(type));
        }

        /// <summary>
        /// Gets a value indicating whether the type implements <see cref="ICollection"/>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the type is a collection; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCollection(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            var typeInfo = IntrospectionExtensions.GetTypeInfo(type);
            var collectionTypeInfo = IntrospectionExtensions.GetTypeInfo(typeof(ICollection<>));
            if (typeInfo.IsConstructedGenericOf(collectionTypeInfo))
            {
                return true;
            }

            return typeInfo.ImplementedInterfaces.Any(i => i.IsCollection());
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
            return type != null
                   && IntrospectionExtensions.GetTypeInfo(typeof(IQueryable))
                       .IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(type));
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
                    var ti = IntrospectionExtensions.GetTypeInfo(t);
                    return ti.IsGenericType && ti.GetGenericTypeDefinition() == enumerableGenericType;
                };
            var enumerableType = isRequestedEnumerable(type)
                                     ? type
                                     : IntrospectionExtensions.GetTypeInfo(type)
                                         .ImplementedInterfaces.SingleOrDefault(isRequestedEnumerable);

            return enumerableType == null
                       ? null
                       : IntrospectionExtensions.GetTypeInfo(enumerableType).GenericTypeArguments[0];
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
            Requires.NotNull(type, nameof(type));

            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == openGenericType;
        }

        /// <summary>
        /// A Type extension method that gets the base constructed generic of a provided type.
        /// The base can be either an interface or a class.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="openGenericType">The open generic type of which constructed generic type is requested.</param>
        /// <returns>
        /// The base constructed generic.
        /// </returns>
        /// <example>
        /// var type = typeof(string).GetBaseConstructedGenericOf(typeof(IEnumerable&lt;&gt;));
        /// Assert.AreSame(type, typeof(IEnumerable&lt;char&gt;));
        /// </example>
        public static Type GetBaseConstructedGenericOf(this Type type, Type openGenericType)
        {
            Requires.NotNull(type, nameof(type));
            Requires.NotNull(openGenericType, nameof(openGenericType));

            var openGenericTypeInfo = IntrospectionExtensions.GetTypeInfo(openGenericType);
            if (openGenericTypeInfo.IsClass)
            {
                while (type != ObjectType)
                {
                    if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == openGenericType)
                    {
                        return type;
                    }

                    type = IntrospectionExtensions.GetTypeInfo(type).BaseType;
                }
            }
            else if (openGenericTypeInfo.IsInterface)
            {
                var implementedInterfaces = IntrospectionExtensions.GetTypeInfo(type).ImplementedInterfaces;
                var constructedInterface = implementedInterfaces.FirstOrDefault(
                    t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == openGenericType);
                return constructedInterface;
            }

            return null;
        }

        /// <summary>
        /// Gets the qualified full name of a <see cref="Type"/>. Optionally, strips away the version information.
        /// </summary>
        /// <param name="type">The type instance.</param>
        /// <param name="stripVersionInfo"><c>true</c> to strip away the version information (optional).</param>
        /// <returns>
        /// The qualified full name.
        /// </returns>
        public static string GetQualifiedFullName(this Type type, bool stripVersionInfo = true)
        {
            Requires.NotNull(type, nameof(type));

            return TypeInfoExtensions.GetQualifiedFullName(IntrospectionExtensions.GetTypeInfo(type), stripVersionInfo);
        }
    }
}