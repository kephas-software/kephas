// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderExportDescriptorProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Factory export descriptor provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ExportProviders
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;
    using System.Diagnostics.Contracts;

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
        /// Stores information about the support for services.
        /// </summary>
        private readonly ConcurrentDictionary<Type, bool> servicesSupport = new ConcurrentDictionary<Type, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderExportDescriptorProvider" />
        /// class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ServiceProviderExportDescriptorProvider(IServiceProvider serviceProvider)
        {
            Contract.Requires(serviceProvider != null);

            this.serviceProvider = serviceProvider;

            // exclude the ICompositionContext service, this will be registered by the respective container.
            this.servicesSupport[typeof(ICompositionContext)] = false;
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
            var isSupported = this.servicesSupport.GetOrAdd(
                contract.ContractType,
                type => this.serviceProvider.GetService(type) != null);

            if (!isSupported)
            {
                return ExportDescriptorProvider.NoExportDescriptors;
            }

            return new[]
               {
                 new ExportDescriptorPromise(
                   contract,
                   contract.ContractType.Name,
                   true, // is shared
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