// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesRegistry.cs" company="Kephas Software SRL">
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
    using Kephas.Diagnostics.Contracts;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// An application service contracts model registry.
    /// </summary>
    public class AppServicesRegistry : IRuntimeModelRegistry
    {
        private readonly IRuntimeTypeRegistry typeRegistry;
        private readonly Func<(Type contractType, IAppServiceInfo appServiceInfo), IAmbientServices, bool>? filter;

        /// <summary>
        /// The key for the app service metadata.
        /// </summary>
        internal static readonly string AppServiceKey = "Kephas:AppService";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServicesRegistry"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="typeRegistry">The runtime type registry.</param>
        public AppServicesRegistry(IAmbientServices ambientServices, IRuntimeTypeRegistry typeRegistry)
            : this(ambientServices, typeRegistry, IsNotThirdParty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServicesRegistry"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="filter">Optional. Sets the filter for eligible service contracts.</param>
        protected internal AppServicesRegistry(
            IAmbientServices ambientServices,
            IRuntimeTypeRegistry typeRegistry,
            Func<(Type contractType, IAppServiceInfo appServiceInfo), IAmbientServices, bool>? filter)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            this.AmbientServices = ambientServices;
            this.typeRegistry = typeRegistry;
            this.filter = filter;
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        protected IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets the runtime elements.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of runtime elements.
        /// </returns>
        public Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default)
        {
            var appServiceInfos = this.AmbientServices.GetAppServiceInfos();
            if (this.filter != null)
            {
                appServiceInfos = appServiceInfos.Where(sc => this.filter(sc, this.AmbientServices));
            }

            var types = new HashSet<IRuntimeTypeInfo>(appServiceInfos.Select(i =>
                {
                    var typeInfo = this.typeRegistry.GetTypeInfo(i.contractType);
                    typeInfo[AppServiceKey] = i.appServiceInfo;
                    return typeInfo;
                }));

            return Task.FromResult<IEnumerable<object>>(types);
        }

        /// <summary>
        /// Gets a value indicating whether the service contract is not part of a third party assembly.
        /// </summary>
        /// <param name="serviceContract">The service contract to be tested.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>A value indicating whether the service contract is not part of a third party assembly.</returns>
        protected static bool IsNotThirdParty((Type contractType, IAppServiceInfo appServiceInfo) serviceContract, IAmbientServices ambientServices)
            => ambientServices.AppRuntime.GetAppAssemblies().Contains(serviceContract.contractType.Assembly);
    }
}