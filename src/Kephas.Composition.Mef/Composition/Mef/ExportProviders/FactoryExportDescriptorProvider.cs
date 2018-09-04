// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryExportDescriptorProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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

    using Kephas.Composition.Mef.Hosting;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Factory export descriptor provider.
    /// </summary>
    public class FactoryExportDescriptorProvider : ExportDescriptorProvider, IExportProvider
    {
        /// <summary>
        /// The factory.
        /// </summary>
        private readonly Func<object> factory;

        /// <summary>
        /// The context factory.
        /// </summary>
        private readonly Func<ICompositionContext, object> contextFactory;

        /// <summary>
        /// The is shared.
        /// </summary>
        private readonly bool isShared;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryExportDescriptorProvider" /> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="factory">The value factory.</param>
        public FactoryExportDescriptorProvider(Type contractType, Func<object> factory)
            : this(contractType, factory, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryExportDescriptorProvider" /> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="factory">The value factory.</param>
        /// <param name="isShared">If set to <c>true</c> the factory provides a shared instance.</param>
        public FactoryExportDescriptorProvider(Type contractType, Func<object> factory, bool isShared)
        {
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(factory, nameof(factory));

            this.ContractType = contractType;
            this.factory = factory;
            this.isShared = isShared;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryExportDescriptorProvider" /> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contextFactory">The value factory.</param>
        /// <param name="isShared">If set to <c>true</c> the factory provides a shared instance.</param>
        public FactoryExportDescriptorProvider(Type contractType, Func<ICompositionContext, object> contextFactory, bool isShared)
        {
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(contextFactory, nameof(contextFactory));

            this.ContractType = contractType;
            this.contextFactory = contextFactory;
            this.isShared = isShared;
        }

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <value>
        /// The type of the contract.
        /// </value>
        public Type ContractType { get; }

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
            if (contract.ContractType != this.ContractType)
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
                                    var instance = this.factory == null
                                                       ? this.contextFactory(MefCompositionContextBase.TryGetCompositionContext(c))
                                                       : this.factory();
                                    if (instance is IDisposable disposable)
                                    {
                                        c.AddBoundInstance(disposable);
                                    }

                                    return instance;
                                },
                        ExportDescriptorProvider.NoMetadata))
               };
        }
    }

    /// <summary>
    /// Factory export descriptor provider.
    /// </summary>
    /// <typeparam name="TContract">The type of the contract.</typeparam>
    public class FactoryExportDescriptorProvider<TContract> : FactoryExportDescriptorProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryExportDescriptorProvider{TContract}" /> class.
        /// </summary>
        /// <param name="factory">The value factory.</param>
        public FactoryExportDescriptorProvider(Func<TContract> factory)
            : base(typeof(TContract), () => factory(), false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryExportDescriptorProvider{TContract}" /> class.
        /// </summary>
        /// <param name="factory">The value factory.</param>
        /// <param name="isShared">If set to <c>true</c> the factory provides a shared instance.</param>
        public FactoryExportDescriptorProvider(Func<TContract> factory, bool isShared)
            : base(typeof(TContract), () => factory(), isShared)
        {
        }
    }
}