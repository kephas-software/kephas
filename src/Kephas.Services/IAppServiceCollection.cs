// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Provides a registration container for application services.
    /// </summary>
    public interface IAppServiceCollection : IExpando, IEnumerable<IAppServiceInfo>
    {
        private static readonly List<Action<IAppServiceCollection>> Collectors = new ();

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="contractType">Type of the service contract.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        public bool Contains(Type contractType)
            => this.Any(r => r.ContractType == contractType);

        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <param name="appServiceInfo">The application service registration.</param>
        /// <returns>
        /// This <see cref="IAppServiceCollection"/>.
        /// </returns>
        public IAppServiceCollection Add(IAppServiceInfo appServiceInfo);

        /// <summary>
        /// Replaces the service with the provided contract.
        /// </summary>
        /// <param name="appServiceInfo">The application service registration.</param>
        /// <returns>
        /// This <see cref="IAppServiceCollection"/>.
        /// </returns>
        public IAppServiceCollection Replace(IAppServiceInfo appServiceInfo);

        /// <summary>
        /// Adds an collector for <see cref="IAppServiceCollection"/>.
        /// </summary>
        /// <param name="collect">The collect callback function.</param>
        public static void AddCollector(Action<IAppServiceCollection> collect)
        {
            lock (Collectors)
            {
                Collectors.Add(collect ?? throw new ArgumentNullException(nameof(collect)));
            }
        }

        /// <summary>
        /// Initializes the ambient services with the registered initializers.
        /// </summary>
        /// <param name="appServices">The application services.</param>
        public static void Initialize(IAppServiceCollection appServices)
        {
            lock (Collectors)
            {
                foreach (var collect in Collectors)
                {
                    collect(appServices);
                }
            }
        }
    }
}