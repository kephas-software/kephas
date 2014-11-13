// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeAffineExportDescriptorProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The type affine export descriptor provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ExportProviders
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Export provider for type affine services.
    /// </summary>
    public class TypeAffineExportDescriptorProvider : ExportDescriptorProvider
    {
        /// <summary>
        /// The logger factory.
        /// </summary>
        private readonly Func<string, object> factory;

        /// <summary>
        /// The service type.
        /// </summary>
        private readonly Type serviceType;

        /// <summary>
        /// The promises cache.
        /// </summary>
        private readonly ConcurrentDictionary<string, IEnumerable<ExportDescriptorPromise>> promisesCache = new ConcurrentDictionary<string, IEnumerable<ExportDescriptorPromise>>();

        /// <summary>
        /// The services cache.
        /// </summary>
        private readonly ConcurrentDictionary<string, object> servicesCache = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAffineExportDescriptorProvider" /> class.
        /// </summary>
        /// <param name="factory">The service factory.</param>
        /// <param name="serviceType">Type of the service.</param>
        public TypeAffineExportDescriptorProvider(Func<string, object> factory, Type serviceType)
        {
            Contract.Requires(factory != null);
            Contract.Requires(serviceType != null);

            this.factory = factory;
            this.serviceType = serviceType;
        }

        /// <summary>
        /// Gets the contract name prefix.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// The contract name prefix.
        /// </returns>
        public static string GetContractNamePrefix(Type serviceType)
        {
            Contract.Requires(serviceType != null);

            return serviceType.FullName + ":";
        }

        /// <summary>
        /// Gets the name of the contract for the provided service and consumer type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="consumerType">Type of the consumer.</param>
        /// <returns>
        /// The name of the contract.
        /// </returns>
        public static string GetContractName(Type serviceType, Type consumerType)
        {
            Contract.Requires(serviceType != null);
            Contract.Requires(consumerType != null);

            var prefix = GetContractNamePrefix(serviceType);
            if (consumerType == null)
            {
                return prefix;
            }

            return prefix + consumerType.FullName;
        }

        /// <summary>
        /// Gets the contract name prefix.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>The contract name prefix.</returns>
        public static string GetContractNamePrefix<TService>()
        {
            return GetContractNamePrefix(typeof(TService));
        }

        /// <summary>
        /// Gets the name of the contract for the provided service and consumer type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="consumerType">Type of the consumer.</param>
        /// <returns>The name of the contract.</returns>
        public static string GetContractName<TService>(Type consumerType)
        {
            Contract.Requires(consumerType != null);

            return GetContractName(typeof(TService), consumerType);
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
            var prefix = GetContractNamePrefix(this.serviceType);
            if (string.IsNullOrEmpty(contract.ContractName) || !contract.ContractName.StartsWith(prefix))
            {
                return ExportDescriptorProvider.NoExportDescriptors;
            }

            var instanceName = contract.ContractName.Substring(prefix.Length);
            return this.promisesCache.GetOrAdd(
              instanceName,
              name => new[]
                  {
                    new ExportDescriptorPromise(
                      contract,
                      contract.ContractName,
                      false,
                      ExportDescriptorProvider.NoDependencies,
                      dependencies => ExportDescriptor.Create(
                        (c, o) =>
                          {
                            var instance = this.servicesCache.GetOrAdd(name, this.factory(instanceName));

                            // do not add a bound instance for disposing even if the
                            // service is disposable, because being type affine
                            // there is no risk to run into memory leaks
                            return instance;
                          }, 
                        ExportDescriptorProvider.NoMetadata))
                  });
        }
    }

    /// <summary>
    /// Export provider for type affine services.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class TypeAffineExportDescriptorProvider<TService> : TypeAffineExportDescriptorProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAffineExportDescriptorProvider{TService}"/> class.
        /// </summary>
        /// <param name="factory">The service factory.</param>
        public TypeAffineExportDescriptorProvider(Func<string, TService> factory)
            : base(name => factory(name), typeof(TService))
        {
            Contract.Requires(factory != null);
        }
    }
}