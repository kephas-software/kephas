﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Services.AttributedModel;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Collection of application service registrations.
    /// </summary>
    [ExcludeFromServices]
    public class AppServiceCollection : Expando, IAppServiceCollection
    {
        private readonly IList<IAppServiceInfo> registry = new List<IAppServiceInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceCollection"/> class.
        /// </summary>
        public AppServiceCollection()
        {
            this.Add<IAppServiceCollection>(this, b => b.ExternallyOwned());
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
        /// This <see cref="IAppServiceCollection"/>.
        /// </returns>
        public IAppServiceCollection Add(IAppServiceInfo appServiceInfo)
        {
            this.registry.Add(appServiceInfo ?? throw new ArgumentNullException(nameof(appServiceInfo)));

            return this;
        }

        /// <summary>
        /// Replaces the service with the provided contract.
        /// </summary>
        /// <param name="appServiceInfo">The application service registration.</param>
        /// <returns>
        /// This <see cref="IAppServiceCollection"/>.
        /// </returns>
        public IAppServiceCollection Replace(IAppServiceInfo appServiceInfo)
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

            this.Add(appServiceInfo);

            return this;
        }
    }
}