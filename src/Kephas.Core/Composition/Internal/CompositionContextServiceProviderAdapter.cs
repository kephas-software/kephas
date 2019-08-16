// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContextServiceProviderAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition context service provider adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    /// <summary>
    /// Adapter for <see cref="IServiceProvider"/> based on a composition context.
    /// </summary>
    internal class CompositionContextServiceProviderAdapter : IServiceProvider
    {
        private static MethodInfo ToEnumerableMethod = ReflectionHelper.GetGenericMethodOf(
            _ => ((CompositionContextServiceProviderAdapter)null).ToEnumerable<int>(null));

        private static MethodInfo ToListMethod = ReflectionHelper.GetGenericMethodOf(
            _ => ((CompositionContextServiceProviderAdapter)null).ToList<int>(null));

        /// <summary>
        /// Context for the composition.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContextServiceProviderAdapter"/>
        /// class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        public CompositionContextServiceProviderAdapter(ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            this.compositionContext = compositionContext;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType" />.-or- null if there is no service
        /// object of type <paramref name="serviceType" />.
        /// </returns>
        public object GetService(Type serviceType)
        {
            if (serviceType.IsConstructedGenericOf(typeof(IExportFactory<>)))
            {
                var contractType = serviceType.GenericTypeArguments[0];
                return this.compositionContext.GetExportFactory(contractType);
            }

            if (serviceType.IsConstructedGenericOf(typeof(IExportFactory<,>)))
            {
                var contractType = serviceType.GenericTypeArguments[0];
                var metadataType = serviceType.GenericTypeArguments[1];
                return this.compositionContext.GetExportFactory(contractType, metadataType);
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
                        return this.compositionContext.GetExportFactories(contractType);
                    }

                    if (exportType.IsConstructedGenericOf(typeof(IExportFactory<,>)))
                    {
                        var contractType = exportType.GenericTypeArguments[0];
                        var metadataType = exportType.GenericTypeArguments[1];
                        return this.compositionContext.GetExportFactories(contractType, metadataType);
                    }

                    var exports = this.compositionContext.GetExports(exportType);
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

            return this.compositionContext.GetExport(serviceType);
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