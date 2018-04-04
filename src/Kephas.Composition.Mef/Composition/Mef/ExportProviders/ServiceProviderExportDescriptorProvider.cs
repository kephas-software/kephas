// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderExportDescriptorProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Factory export descriptor provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ExportProviders
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Factory export descriptor provider based on a <see cref="IServiceProvider"/>.
    /// </summary>
    public class ServiceProviderExportDescriptorProvider : ExportDescriptorProvider, IExportProvider
    {
        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Function used to query whether the service provider registers a specific service.
        /// </summary>
        private readonly Func<IServiceProvider, Type, bool> isServiceRegisteredFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderExportDescriptorProvider" />
        /// class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="isServiceRegisteredFunc">Function used to query whether the service provider registers a specific service.</param>
        public ServiceProviderExportDescriptorProvider(IServiceProvider serviceProvider, Func<IServiceProvider, Type, bool> isServiceRegisteredFunc)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            Requires.NotNull(isServiceRegisteredFunc, nameof(isServiceRegisteredFunc));

            this.serviceProvider = serviceProvider;
            this.isServiceRegisteredFunc = isServiceRegisteredFunc;
        }

        /// <summary>
        /// Promise export descriptors for the specified export key.
        /// </summary>
        /// <param name="contract">The export key required by another component.</param><param name="descriptorAccessor">Accesses the other export descriptors present in the composition.</param>
        /// <returns>
        /// Promises for new export descriptors.
        /// </returns>
        /// <remarks>
        /// A provider will only be queried once for each unique export key.
        ///             The descriptor accessor can only be queried immediately if the descriptor being promised is an adapter, such as
        ///             <see cref="T:System.Lazy`1"/>; otherwise, dependencies should only be queried within execution of the function provided
        ///             to the <see cref="T:System.Composition.Hosting.Core.ExportDescriptorPromise"/>. The actual descriptors provided should not close over or reference any
        ///             aspect of the dependency/promise structure, as this should be able to be GC'ed.
        /// </remarks>
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            // exclude the ICompositionContext service, this will be registered by the respective container.
            if (contract.ContractType == typeof(ICompositionContext) ||
                !this.isServiceRegisteredFunc(this.serviceProvider, contract.ContractType))
            {
                return ExportDescriptorProvider.NoExportDescriptors;
            }

            return new[]
               {
                 new ExportDescriptorPromise(
                   contract,
                   contract.ContractType.Name,
                   false, // is not shared, make it each time query for the service, because of the service factory registration.
                   ExportDescriptorProvider.NoDependencies,
                   dependencies => ExportDescriptor.Create(
                     (c, o) =>
                        {
                            var instance = this.serviceProvider.GetService(contract.ContractType);
                            var disposable = instance as IDisposable;
                            if (disposable != null)
                            {
                                c.AddBoundInstance(disposable);
                            }

                            return instance;
                        },
                     ExportDescriptorProvider.NoMetadata))
               };
        }
    }
}