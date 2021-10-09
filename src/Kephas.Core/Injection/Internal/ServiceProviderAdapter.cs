// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    /// <summary>
    /// Adapter for <see cref="IServiceProvider"/> based on <see cref="IInjector"/>.
    /// </summary>
    internal class ServiceProviderAdapter : IServiceProvider
    {
        private static readonly MethodInfo ToEnumerableMethod = ReflectionHelper.GetGenericMethodOf(
            _ => ((ServiceProviderAdapter)null).ToEnumerable<int>(null));

        private static readonly MethodInfo ToListMethod = ReflectionHelper.GetGenericMethodOf(
            _ => ((ServiceProviderAdapter)null).ToList<int>(null));

        private readonly IInjector injector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAdapter"/>
        /// class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        public ServiceProviderAdapter(IInjector injector)
        {
            injector = injector ?? throw new ArgumentNullException(nameof(injector));

            this.injector = injector;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType" />.-or- null if there is no service
        /// object of type <paramref name="serviceType" />.
        /// </returns>
        public object? GetService(Type serviceType)
        {
            if (serviceType.IsConstructedGenericOf(typeof(IExportFactory<>)))
            {
                var contractType = serviceType.GenericTypeArguments[0];
                return this.injector.GetExportFactory(contractType);
            }

            if (serviceType.IsConstructedGenericOf(typeof(IExportFactory<,>)))
            {
                var contractType = serviceType.GenericTypeArguments[0];
                var metadataType = serviceType.GenericTypeArguments[1];
                return this.injector.GetExportFactory(contractType, metadataType);
            }

            if (serviceType.IsConstructedGenericOf(typeof(IEnumerable<>)) ||
                serviceType.IsConstructedGenericOf(typeof(ICollection<>)) ||
                serviceType.IsConstructedGenericOf(typeof(IList<>)) ||
                serviceType.IsConstructedGenericOf(typeof(List<>)))
            {
                var exportType = serviceType.TryGetEnumerableItemType();
                if (exportType != null)
                {
                    if (exportType.IsConstructedGenericOf(typeof(IExportFactory<>)))
                    {
                        var contractType = exportType.GenericTypeArguments[0];
                        return this.injector.GetExportFactories(contractType);
                    }

                    if (exportType.IsConstructedGenericOf(typeof(IExportFactory<,>)))
                    {
                        var contractType = exportType.GenericTypeArguments[0];
                        var metadataType = exportType.GenericTypeArguments[1];
                        return this.injector.GetExportFactories(contractType, metadataType);
                    }

                    var exports = this.injector.ResolveMany(exportType);
                    if (serviceType.IsClass)
                    {
                        var toList = ToListMethod.MakeGenericMethod(exportType);
                        return toList.Call(this, exports);
                    }
                    else
                    {
                        var toEnumerable = ToEnumerableMethod.MakeGenericMethod(serviceType);
                        return toEnumerable.Call(this, exports);
                    }
                }
            }

            return this.injector.TryResolve(serviceType);
        }

        private TEnumerable ToEnumerable<TEnumerable>(IEnumerable<object> exports)
        {
            return (TEnumerable)exports;
        }

        private List<TItem> ToList<TItem>(IEnumerable<object> exports)
        {
            return exports.Cast<TItem>().ToList();
        }
    }
}