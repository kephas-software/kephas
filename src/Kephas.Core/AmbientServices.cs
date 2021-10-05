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
    using System.Collections.Generic;

    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Lite.Builder;
    using Kephas.Injection.Lite.Internal;
    using Kephas.Licensing;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Provides the global ambient services.
    /// </summary>
    /// <remarks>
    /// It is a recommended practice to not use global services, instead get the services
    /// using the composition (the classical example is the unit testing, where the classes
    /// should be sandboxed as much as possible). However, there may be cases when this cannot be avoided,
    /// such as static classes or classes which get instantiated outside of the developer's control
    /// (like in the case of the entities instantiated by the ORMs). Those are cases where the
    /// <see cref="AmbientServices"/> can be safely used.
    /// </remarks>
    [ExcludeFromInjection]
    public class AmbientServices : Expando, IAmbientServices, IAppServiceInfosProvider
    {
        private readonly IAppServiceRegistry registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        /// <param name="registerDefaultServices">Optional. True to register default services.</param>
        /// <param name="typeRegistry">Optional. The type registry.</param>
        public AmbientServices(bool registerDefaultServices = true, IRuntimeTypeRegistry? typeRegistry = null)
        {
            this.registry = new ServiceRegistry();

            typeRegistry ??= RuntimeTypeRegistry.Instance;

            this.Register<IAmbientServices>(b => b.WithInstance(this).ExternallyOwned(true))
                .Register<IInjector>(b => b.WithInstance(this.AsInjector()).ExternallyOwned(true))
                .Register<IRuntimeTypeRegistry>(typeRegistry);

            if (registerDefaultServices)
            {
                this
                    .Register<IConfigurationStore, DefaultConfigurationStore>()
                    .Register<ILogManager, NullLogManager>()
                    .Register<ITypeLoader, DefaultTypeLoader>()
                    .Register<ILicensingManager, NullLicensingManager>();
            }

            this.registry
                .RegisterSource(new LazyServiceSource(this.registry, typeRegistry))
                .RegisterSource(new LazyWithMetadataServiceSource(this.registry, typeRegistry))
                .RegisterSource(new ExportFactoryServiceSource(this.registry, typeRegistry))
                .RegisterSource(new ExportFactoryWithMetadataServiceSource(this.registry, typeRegistry))
                .RegisterSource(new ListServiceSource(this.registry, typeRegistry))
                .RegisterSource(new CollectionServiceSource(this.registry, typeRegistry))
                .RegisterSource(new EnumerableServiceSource(this.registry, typeRegistry));
        }

        /// <summary>
        /// Gets the service registry.
        /// </summary>
        IAppServiceRegistry IAmbientServices.ServiceRegistry => this.registry;

        /// <summary>
        /// Gets the application service infos in this collection.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the application service infos in this
        /// collection.
        /// </returns>
        public IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(dynamic? context = null)
        {
            // Lite injector does not need to add to ambient services again its services
            // However, when the registration context and the candidate types are both null,
            // this is a message that ALL registration infos should be returned.
            if (context != null && ((bool?)this[LiteInjectorBuilder.LiteInjectionKey] ?? false))
            {
                yield break;
            }

            // exclude the injector from the list as it is the responsibility
            // of each injector implementation to register itself in the DI container.
            foreach (var s in this.registry.GetAppServiceInfos(context))
            {
                if (ReferenceEquals(s.contractDeclarationType, typeof(IInjector)))
                {
                    continue;
                }

                yield return s;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.registry?.Dispose();
        }
    }
}