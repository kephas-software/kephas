// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides the global ambient services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Kephas.Application;
    using Kephas.Dynamic;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.AttributedModel;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Provides the global ambient services.
    /// </summary>
    [ExcludeFromServices]
    public class AmbientServices : Expando, IAmbientServices
    {
        private readonly IList<IAppServiceInfo> registry = new List<IAppServiceInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        public AmbientServices()
            : this(registerDefaultServices: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        /// <param name="registerDefaultServices">Optional. True to register default services.</param>
        protected internal AmbientServices(bool registerDefaultServices)
        {
#if NETSTANDARD2_1
            // for versions prior to .NET 6.0 make sure that the assemblies are initialized.
            IAssemblyInitializer.InitializeAssemblies();
#endif
            if (registerDefaultServices)
            {
                IAmbientServices.Initialize(this);
            }

            this.Add<IAmbientServices>(this, b => b.ExternallyOwned());
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IAppServiceInfo> GetEnumerator() => this.registry.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <param name="appServiceInfo">The application service registration.</param>
        /// <returns>
        /// This <see cref="IAmbientServices"/>.
        /// </returns>
        public IAmbientServices Add(IAppServiceInfo appServiceInfo)
        {
            this.registry.Add(appServiceInfo ?? throw new ArgumentNullException(nameof(appServiceInfo)));

            return this;
        }

        /// <summary>
        /// Replaces the service with the provided contract.
        /// </summary>
        /// <param name="appServiceInfo">The application service registration.</param>
        /// <returns>
        /// This <see cref="IAmbientServices"/>.
        /// </returns>
        public IAmbientServices Replace(IAppServiceInfo appServiceInfo)
        {
            var toDelete = this.registry
                .Select((i, idx) => (appServiceInfo: i, index: idx))
                .Where(t => t.appServiceInfo.ContractType == appServiceInfo.ContractType)
                .Select(t => t.index)
                .OrderBy(i => i)
                .ToList();
            var delta = 0;
            foreach (var i in toDelete)
            {
                this.registry.RemoveAt(i + (delta--));
            }

            return this;
        }
    }
}