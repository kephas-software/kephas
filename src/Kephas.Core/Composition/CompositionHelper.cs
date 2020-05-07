// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Kephas.Data;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// A composition helper.
    /// </summary>
    public static class CompositionHelper
    {
        /// <summary>
        /// Converts this collection of service factories to a dictionary.
        /// </summary>
        /// <remarks>
        /// If, for the same key, there are multiple matching services, their override priority and
        /// processing priority is considered, in this order. Further, if both of these priorities are
        /// equal, then a <see cref="DuplicateKeyException"/> occurs.
        /// </remarks>
        /// <exception cref="DuplicateKeyException">Thrown when a Duplicate Key error condition occurs.</exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceMetadata">Type of the service metadata.</typeparam>
        /// <typeparam name="TKey">Type of the dictionary key.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary value.</typeparam>
        /// <param name="serviceFactories">The service factories.</param>
        /// <param name="keyFunc">The key function.</param>
        /// <param name="valueFunc">The value function.</param>
        /// <param name="keyComparer">Optional. The key comparer.</param>
        /// <returns>
        /// The given data converted to a IDictionary&lt;TKey,TValue&gt;.
        /// </returns>
        public static IDictionary<TKey, TValue> ToPrioritizedDictionary<TService, TServiceMetadata, TKey, TValue>(
            this IEnumerable<IExportFactory<TService, TServiceMetadata>> serviceFactories,
            Func<IExportFactory<TService, TServiceMetadata>, TKey> keyFunc,
            Func<IExportFactory<TService, TServiceMetadata>, TValue> valueFunc,
            IEqualityComparer<TKey>? keyComparer = null)
            where TServiceMetadata : AppServiceMetadata
        {
            Requires.NotNull(serviceFactories, nameof(serviceFactories));
            Requires.NotNull(keyFunc, nameof(keyFunc));
            Requires.NotNull(valueFunc, nameof(valueFunc));

            return serviceFactories
                .Order()
                .ToPrioritizedDictionary<TService, TServiceMetadata, TKey, TValue, IExportFactory<TService, TServiceMetadata>>(
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
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceMetadata">Type of the service metadata.</typeparam>
        /// <typeparam name="TKey">Type of the dictionary key.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary value.</typeparam>
        /// <param name="serviceFactories">The service factories.</param>
        /// <param name="keyFunc">The key function.</param>
        /// <param name="valueFunc">The value function.</param>
        /// <param name="keyComparer">Optional. The key comparer.</param>
        /// <returns>
        /// The given data converted to a IDictionary&lt;TKey,TValue&gt;.
        /// </returns>
        public static IDictionary<TKey, TValue> ToPrioritizedDictionary<TService, TServiceMetadata, TKey, TValue>(
            this IEnumerable<Lazy<TService, TServiceMetadata>> serviceFactories,
            Func<Lazy<TService, TServiceMetadata>, TKey> keyFunc,
            Func<Lazy<TService, TServiceMetadata>, TValue> valueFunc,
            IEqualityComparer<TKey>? keyComparer = null)
            where TServiceMetadata : AppServiceMetadata
        {
            Requires.NotNull(serviceFactories, nameof(serviceFactories));
            Requires.NotNull(keyFunc, nameof(keyFunc));
            Requires.NotNull(valueFunc, nameof(valueFunc));

            return serviceFactories
                .Order()
                .ToPrioritizedDictionary<TService, TServiceMetadata, TKey, TValue, Lazy<TService, TServiceMetadata>>(
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
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceMetadata">Type of the service metadata.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="serviceFactories">The service factories.</param>
        /// <param name="keyFunc">The key function.</param>
        /// <param name="keyComparer">Optional. The key comparer.</param>
        /// <returns>
        /// The given data converted to an IDictionary&lt;TKey,TValue&gt;.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDictionary<TKey, IExportFactory<TService, TServiceMetadata>> ToPrioritizedDictionary<TService, TServiceMetadata, TKey>(
            this IEnumerable<IExportFactory<TService, TServiceMetadata>> serviceFactories,
            Func<IExportFactory<TService, TServiceMetadata>, TKey> keyFunc,
            IEqualityComparer<TKey>? keyComparer = null)
            where TServiceMetadata : AppServiceMetadata
        {
            return serviceFactories
                .Order()
                .ToPrioritizedDictionary<TService, TServiceMetadata, TKey, IExportFactory<TService, TServiceMetadata>, IExportFactory<TService, TServiceMetadata>>(
                    f => f.Metadata,
                    keyFunc,
                    f => f,
                    keyComparer);
            return serviceFactories.ToPrioritizedDictionary(keyFunc, f => f, keyComparer);
        }

        /// <summary>
        /// Converts this collection of service factories to a dictionary.
        /// </summary>
        /// <remarks>
        /// If, for the same key, there are multiple matching services, their override priority and
        /// processing priority is considered, in this order. Further, if both of these priorities are
        /// equal, then a <see cref="DuplicateKeyException"/> occurs.
        /// </remarks>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceMetadata">Type of the service metadata.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="serviceFactories">The service factories.</param>
        /// <param name="keyFunc">The key function.</param>
        /// <param name="keyComparer">Optional. The key comparer.</param>
        /// <returns>
        /// The given data converted to an IDictionary&lt;TKey,TValue&gt;.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDictionary<TKey, Lazy<TService, TServiceMetadata>> ToPrioritizedDictionary<TService, TServiceMetadata, TKey>(
            this IEnumerable<Lazy<TService, TServiceMetadata>> serviceFactories,
            Func<Lazy<TService, TServiceMetadata>, TKey> keyFunc,
            IEqualityComparer<TKey>? keyComparer = null)
            where TServiceMetadata : AppServiceMetadata
        {
            return serviceFactories
                .Order()
                .ToPrioritizedDictionary<TService, TServiceMetadata, TKey, Lazy<TService, TServiceMetadata>, Lazy<TService, TServiceMetadata>>(
                    f => f.Metadata,
                    keyFunc,
                    f => f,
                    keyComparer);
        }

        private static IDictionary<TKey, TValue> ToPrioritizedDictionary<TService, TServiceMetadata, TKey, TValue, T>(
            this IEnumerable<T> serviceFactories,
            Func<T, TServiceMetadata> metadataFunc,
            Func<T, TKey> keyFunc,
            Func<T, TValue> valueFunc,
            IEqualityComparer<TKey>? keyComparer = null)
            where TServiceMetadata : AppServiceMetadata
        {
            Requires.NotNull(serviceFactories, nameof(serviceFactories));
            Requires.NotNull(keyFunc, nameof(keyFunc));
            Requires.NotNull(valueFunc, nameof(valueFunc));

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
                    if (g0meta.OverridePriority == g1meta.OverridePriority
                        && g0meta.ProcessingPriority == g1meta.ProcessingPriority)
                    {
                        throw new DuplicateKeyException("Key", string.Format(Strings.CompositionHelper_ToDictionary_CannotResolveServicePriority_Exception, typeof(TService), g0meta, g1meta));
                    }

                    dictionary.Add(key, valueFunc(group[0].factory));
                }
            }

            return dictionary;
        }
    }
}