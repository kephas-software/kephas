﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The MEF composition container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Hosting;
    using System.Diagnostics.Contracts;

    using Kephas.Composition;
    using Kephas.Composition.Mef.ExportProviders;

    /// <summary>
    /// The MEF composition container.
    /// </summary>
    public class CompositionContainer : ICompositionContainer, IDisposable
    {
        /// <summary>
        /// The inner container.
        /// </summary>
        private CompositionHost innerContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContainer" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        internal CompositionContainer(ContainerConfiguration configuration)
        {
            Contract.Requires(configuration != null);

            configuration.WithProvider(new FactoryExportDescriptorProvider<ICompositionContainer>(() => this, isShared: true));

            this.innerContainer = configuration.CreateContainer();
        }

        /// <summary>
        /// Composes the parts without registering them for recomposition.
        /// </summary>
        /// <param name="parts">The parts to be composed.</param>
        public void SatisfyImports(params object[] parts)
        {
            if (parts == null || parts.Length == 0)
            {
                return;
            }

            foreach (var part in parts)
            {
                this.innerContainer.SatisfyImports(part);
            }
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An object implementing <paramref name="contractType"/>.</returns>
        public object GetExport(Type contractType, string contractName = null)
        {
            var component = string.IsNullOrEmpty(contractName)
                              ? this.innerContainer.GetExport(contractType)
                              : this.innerContainer.GetExport(contractType, contractName);
            return component;
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An enumeration of objects implementing <paramref name="contractType"/>.</returns>
        public IEnumerable<object> GetExports(Type contractType, string contractName = null)
        {
            var components = string.IsNullOrEmpty(contractName)
                              ? this.innerContainer.GetExports(contractType)
                              : this.innerContainer.GetExports(contractType, contractName);
            return components;
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An object implementing <paramref name="{T}" />.
        /// </returns>
        public T GetExport<T>(string contractName = null)
        {
            var component = string.IsNullOrEmpty(contractName)
                              ? this.innerContainer.GetExport<T>()
                              : this.innerContainer.GetExport<T>(contractName);
            return component;
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An enumeration of objects implementing <paramref name="{T}" />.
        /// </returns>
        public IEnumerable<T> GetExports<T>(string contractName = null)
        {
            var components = string.IsNullOrEmpty(contractName)
                              ? this.innerContainer.GetExports<T>()
                              : this.innerContainer.GetExports<T>(contractName);
            return components;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.innerContainer != null)
            {
                this.innerContainer.Dispose();
                this.innerContainer = null;
            }
        }
    }
}