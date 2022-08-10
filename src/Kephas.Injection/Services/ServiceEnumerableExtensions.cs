// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceEnumerableExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ordered service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Kephas.Data;
    using Kephas.Injection;
    using Kephas.Resources;

    /// <summary>
    /// An ordered service collection extensions.
    /// </summary>
    public static class ServiceEnumerableExtensions
    {
        /// <summary>
        /// Orders the given factory service collection.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
        /// <param name="factories">The factories to act on.</param>
        /// <returns>
        /// A list of ordered services.
        /// </returns>
        public static IFactoryEnumerable<T, TMetadata> Order<T, TMetadata>(
            this IEnumerable<IExportFactory<T, TMetadata>> factories)
        {
            return new FactoryEnumerable<T, TMetadata>(factories);
        }

        /// <summary>
        /// Orders the given lazy service collection.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
        /// <param name="factories">The factories to act on.</param>
        /// <returns>
        /// A list of ordered services.
        /// </returns>
        public static ILazyEnumerable<T, TMetadata> Order<T, TMetadata>(
            this IEnumerable<Lazy<T, TMetadata>> factories)
        {
            return new LazyEnumerable<T, TMetadata>(factories);
        }

        /// <summary>
        /// Converts this collection of service factories to a dictionary.
        /// </summary>
        /// <remarks>
        /// If, for the same key, there are multiple matching services, their override priority and
        /// processing priority is considered, in this order. Further, if both of these priorities are
        /// equal, then a <see cref="DuplicateKeyException"/> occurs.
        /// </remarks>
        /// <exception cref="DuplicateKeyException">Thrown when a Duplicate Key error condition occurs.</exception>
        /// <typeparam name="T">Type of the service contract.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <typeparam name="TKey">Type of the dictionary key.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary value.</typeparam>
        /// <param name="serviceFactories">The service factories.</param>
        /// <param name="keyFunc">The key function.</param>
        /// <param name="valueFunc">The value function.</param>
        /// <param name="keyComparer">Optional. The key comparer.</param>
        /// <returns>
        /// The given data converted to a IDictionary&lt;TKey,TValue&gt;.
        /// </returns>
        public static IDictionary<TKey, TValue> OrderAsDictionary<T, TMetadata, TKey, TValue>(
            this IEnumerable<IExportFactory<T, TMetadata>> serviceFactories,
            Func<IExportFactory<T, TMetadata>, TKey> keyFunc,
            Func<IExportFactory<T, TMetadata>, TValue> valueFunc,
            IEqualityComparer<TKey>? keyComparer = null)
            where TKey : notnull
        {
            serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));
            keyFunc = keyFunc ?? throw new ArgumentNullException(nameof(keyFunc));
            valueFunc = valueFunc ?? throw new ArgumentNullException(nameof(valueFunc));

            return serviceFactories
                .Order()
                .OrderAsDictionary<T, TMetadata, TKey, TValue, IExportFactory<T, TMetadata>>(
                    f => f.Metadata,
                    keyFunc,
                    valueFunc,
                    keyComparer);
        }

        /// <summary>
        /// Converts this collection of service factories to a dictionary.
        /// </summary>
        /// <remarks>
        /// If, for the same key, there are multiple matching services, their override priority and
        /// processing priority is considered, in this order. Further, if both of these priorities are
        /// equal, then a <see cref="DuplicateKeyException"/> occurs.
        /// </remarks>
        /// <exception cref="DuplicateKeyException">Thrown when a Duplicate Key error condition occurs.</exception>
        /// <typeparam name="T">Type of the service contract.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <typeparam name="TKey">Type of the dictionary key.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary value.</typeparam>
        /// <param name="serviceFactories">The service factories.</param>
        /// <param name="keyFunc">The key function.</param>
        /// <param name="valueFunc">The value function.</param>
        /// <param name="keyComparer">Optional. The key comparer.</param>
        /// <returns>
        /// The given data converted to a IDictionary&lt;TKey,TValue&gt;.
        /// </returns>
        public static IDictionary<TKey, TValue> OrderAsDictionary<T, TMetadata, TKey, TValue>(
            this IEnumerable<Lazy<T, TMetadata>> serviceFactories,
            Func<Lazy<T, TMetadata>, TKey> keyFunc,
            Func<Lazy<T, TMetadata>, TValue> valueFunc,
            IEqualityComparer<TKey>? keyComparer = null)
            where TKey : notnull
        {
            serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));
            keyFunc = keyFunc ?? throw new ArgumentNullException(nameof(keyFunc));
            valueFunc = valueFunc ?? throw new ArgumentNullException(nameof(valueFunc));

            return serviceFactories
                .Order()
                .OrderAsDictionary<T, TMetadata, TKey, TValue, Lazy<T, TMetadata>>(
                    f => f.Metadata,
                    keyFunc,
                    valueFunc,
                    keyComparer);
        }

        /// <summary>
        /// Converts this collection of service factories to a dictionary.
        /// </summary>
        /// <remarks>
        /// If, for the same key, there are multiple matching services, their override priority and
        /// processing priority is considered, in this order. Further, if both of these priorities are
        /// equal, then a <see cref="DuplicateKeyException"/> occurs.
        /// </remarks>
        /// <typeparam name="T">Type of the service contract.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="serviceFactories">The service factories.</param>
        /// <param name="keyFunc">The key function.</param>
        /// <param name="keyComparer">Optional. The key comparer.</param>
        /// <returns>
        /// The given data converted to an IDictionary&lt;TKey,TValue&gt;.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDictionary<TKey, IExportFactory<T, TMetadata>> OrderAsDictionary<T, TMetadata, TKey>(
            this IEnumerable<IExportFactory<T, TMetadata>> serviceFactories,
            Func<IExportFactory<T, TMetadata>, TKey> keyFunc,
            IEqualityComparer<TKey>? keyComparer = null)
            where TKey : notnull
        {
            return serviceFactories
                .Order()
                .OrderAsDictionary<T, TMetadata, TKey, IExportFactory<T, TMetadata>, IExportFactory<T, TMetadata>>(
                    f => f.Metadata,
                    keyFunc,
                    f => f,
                    keyComparer);
        }

        /// <summary>
        /// Converts this collection of service factories to a dictionary.
        /// </summary>
        /// <remarks>
        /// If, for the same key, there are multiple matching services, their override priority and
        /// processing priority is considered, in this order. Further, if both of these priorities are
        /// equal, then a <see cref="DuplicateKeyException"/> occurs.
        /// </remarks>
        /// <typeparam name="T">Type of the service contract.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="serviceFactories">The service factories.</param>
        /// <param name="keyFunc">The key function.</param>
        /// <param name="keyComparer">Optional. The key comparer.</param>
        /// <returns>
        /// The given data converted to an IDictionary&lt;TKey,TValue&gt;.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDictionary<TKey, Lazy<T, TMetadata>> OrderAsDictionary<T, TMetadata, TKey>(
            this IEnumerable<Lazy<T, TMetadata>> serviceFactories,
            Func<Lazy<T, TMetadata>, TKey> keyFunc,
            IEqualityComparer<TKey>? keyComparer = null)
            where TKey : notnull
        {
            return serviceFactories
                .Order()
                .OrderAsDictionary<T, TMetadata, TKey, Lazy<T, TMetadata>, Lazy<T, TMetadata>>(
                    f => f.Metadata,
                    keyFunc,
                    f => f,
                    keyComparer);
        }

        private static IDictionary<TKey, TValue> OrderAsDictionary<T, TMetadata, TKey, TValue, TFactory>(
            this IEnumerable<TFactory> serviceFactories,
            Func<TFactory, TMetadata> metadataFunc,
            Func<TFactory, TKey> keyFunc,
            Func<TFactory, TValue> valueFunc,
            IEqualityComparer<TKey>? keyComparer = null)
            where TKey : notnull
        {
            serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));
            keyFunc = keyFunc ?? throw new ArgumentNullException(nameof(keyFunc));
            valueFunc = valueFunc ?? throw new ArgumentNullException(nameof(valueFunc));

            var dictionary = keyComparer == null ? new Dictionary<TKey, TValue>() : new Dictionary<TKey, TValue>(keyComparer);

            var factoryProjection = serviceFactories
                .Select(f => (key: keyFunc(f), meta: metadataFunc(f), factory: f));
            var orderedFactories = (keyComparer == null
                                       ? factoryProjection.GroupBy(e => e.key)
                                       : factoryProjection.GroupBy(e => e.key, e => e, keyComparer))
                .ToList();

            foreach (var entry in orderedFactories)
            {
                var key = entry.Key;
                var group = entry.ToList();
                if (group.Count == 1)
                {
                    dictionary.Add(key, valueFunc(group[0].factory));
                }
                else
                {
                    var g0meta = group[0].meta;
                    var g1meta = group[1].meta;
                    if (HasSamePriority(g0meta, g1meta))
                    {
                        throw new DuplicateKeyException("Key", string.Format(AbstractionStrings.CompositionHelper_ToDictionary_CannotResolveServicePriority_Exception, typeof(T), g0meta, g1meta));
                    }

                    dictionary.Add(key, valueFunc(group[0].factory));
                }
            }

            return dictionary;
        }

        private static bool HasSamePriority<TMetadata>(TMetadata metadata1, TMetadata metadata2)
        {
            return ((metadata1 as IHasOverridePriority)?.OverridePriority ?? Priority.Normal) == ((metadata2 as IHasOverridePriority)?.OverridePriority ?? Priority.Normal)
                   && ((metadata1 as IHasProcessingPriority)?.ProcessingPriority ?? Priority.Normal) == ((metadata2 as IHasProcessingPriority)?.ProcessingPriority ?? Priority.Normal);
        }
    }
}