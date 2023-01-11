// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesModelRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service contracts serviceRegistry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.ModelRegistries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Application;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// An application service contracts model registry.
    /// </summary>
    public class AppServicesModelRegistry : IRuntimeModelRegistry
    {
        private readonly IRuntimeTypeRegistry typeRegistry;
        private readonly Func<IAppServiceInfo, IAppRuntime, bool>? filter;

        /// <summary>
        /// The key for the app service metadata.
        /// </summary>
        internal static readonly string AppServiceKey = "Kephas:AppService";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServicesModelRegistry"/> class.
        /// </summary>
        /// <param name="appServices">The application services.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeRegistry">The runtime type registry.</param>
        public AppServicesModelRegistry(IAppServiceCollection appServices, IAppRuntime appRuntime, IRuntimeTypeRegistry typeRegistry)
            : this(appServices, appRuntime, typeRegistry, IsNotThirdParty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServicesModelRegistry"/> class.
        /// </summary>
        /// <param name="appServices">The application services.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="filter">Optional. Sets the filter for eligible service contracts.</param>
        protected internal AppServicesModelRegistry(
            IAppServiceCollection appServices,
            IAppRuntime appRuntime,
            IRuntimeTypeRegistry typeRegistry,
            Func<IAppServiceInfo, IAppRuntime, bool>? filter)
        {
            this.AppServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            this.AppRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));
            this.typeRegistry = typeRegistry;
            this.filter = filter;
        }

        /// <summary>
        /// Gets The application services.
        /// </summary>
        protected IAppServiceCollection AppServices { get; }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        protected IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the runtime elements.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of runtime elements.
        /// </returns>
        public Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<IAppServiceInfo> appServiceInfos = this.AppServices;
            if (this.filter != null)
            {
                appServiceInfos = appServiceInfos.Where(sc => this.filter(sc, this.AppRuntime));
            }

            var types = new HashSet<IRuntimeTypeInfo>(appServiceInfos
                .Where(i => i.ContractDeclarationType is not null && i.InstancingStrategy is null)
                .Select(i =>
                {
                    var typeInfo = this.typeRegistry.GetTypeInfo(i.ContractDeclarationType!);
                    typeInfo[AppServiceKey] = i;
                    return typeInfo;
                }));

            return Task.FromResult<IEnumerable<object>>(types);
        }

        /// <summary>
        /// Gets a value indicating whether the service contract is not part of a third party assembly.
        /// </summary>
        /// <param name="appServiceInfo">The service contract to be tested.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>A value indicating whether the service contract is not part of a third party assembly.</returns>
        protected static bool IsNotThirdParty(IAppServiceInfo appServiceInfo, IAppRuntime appRuntime)
            => appServiceInfo.ContractDeclarationType != null && appRuntime.GetAppAssemblies().Contains(appServiceInfo.ContractDeclarationType.Assembly);
    }
}