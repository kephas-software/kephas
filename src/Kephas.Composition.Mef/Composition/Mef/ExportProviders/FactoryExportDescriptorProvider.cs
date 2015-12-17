// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryExportDescriptorProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Factory export descriptor provider.
    /// </summary>
    /// <typeparam name="TContract">The type of the contract.</typeparam>
    public class FactoryExportDescriptorProvider<TContract> : ExportDescriptorProvider, IExportProvider
    {
        /// <summary>
        /// The factory.
        /// </summary>
        private readonly Func<TContract> factory;

        /// <summary>
        /// The is shared.
        /// </summary>
        private readonly bool isShared;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryExportDescriptorProvider{TContract}" /> class.
        /// </summary>
        /// <param name="factory">The value factory.</param>
        public FactoryExportDescriptorProvider(Func<TContract> factory)
            : this(factory, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryExportDescriptorProvider{TContract}" /> class.
        /// </summary>
        /// <param name="factory">The value factory.</param>
        /// <param name="isShared">If set to <c>true</c> the factory provides a shared instance.</param>
        public FactoryExportDescriptorProvider(Func<TContract> factory, bool isShared)
        {
            Contract.Requires(factory != null);

            this.factory = factory;
            this.isShared = isShared;
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
            if (contract.ContractType != typeof(TContract))
            {
                return ExportDescriptorProvider.NoExportDescriptors;
            }

            return new[]
               {
                 new ExportDescriptorPromise(
                   contract,
                   contract.ContractType.Name,
                   this.isShared,
                   ExportDescriptorProvider.NoDependencies,
                   dependencies => ExportDescriptor.Create(
                     (c, o) =>
                       {
                       var instance = this.factory();
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