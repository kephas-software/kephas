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
    using System.Linq;
    using System.Reflection;

    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
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
    public class AmbientServices : Expando, IAmbientServicesMixin, IAppServiceInfosProvider
    {
        private readonly IAppServiceRegistry registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        public AmbientServices()
            : this(registerDefaultServices: true, typeRegistry: null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        public AmbientServices(IRuntimeTypeRegistry? typeRegistry)
            : this(registerDefaultServices: true, typeRegistry: typeRegistry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        /// <param name="registerDefaultServices">Optional. True to register default services.</param>
        /// <param name="typeRegistry">Optional. The type registry.</param>
        protected internal AmbientServices(bool registerDefaultServices, IRuntimeTypeRegistry? typeRegistry)
        {
            this.registry = new ServiceRegistry();

            typeRegistry ??= RuntimeTypeRegistry.Instance;

            this.Register<IAmbientServices>(this, b => b.ExternallyOwned())
                .Register<IInjector>(this.registry.ToInjector(), b => b.ExternallyOwned())
                .Register<IRuntimeTypeRegistry>(typeRegistry, b => b.ExternallyOwned());

            if (registerDefaultServices)
            {
                this
                    .Register<IConfigurationStore, DefaultConfigurationStore>()
                    .Register<ILogManager, NullLogManager>()
                    .Register<ITypeLoader, DefaultTypeLoader>()
                    .Register<ILicensingManager, NullLicensingManager>();
            }

            this.registry
                .RegisterSource(new LazyServiceSource(this.registry))
                .RegisterSource(new LazyWithMetadataServiceSource(this.registry))
                .RegisterSource(new ExportFactoryServiceSource(this.registry))
                .RegisterSource(new ExportFactoryWithMetadataServiceSource(this.registry))
                .RegisterSource(new ListServiceSource(this.registry))
                .RegisterSource(new CollectionServiceSource(this.registry))
                .RegisterSource(new EnumerableServiceSource(this.registry));
        }

        /// <summary>
        /// Gets the service registry.
        /// </summary>
        IAppServiceRegistry IAmbientServicesMixin.ServiceRegistry => this.registry;

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <returns>An enumeration of application assemblies.</returns>
        public IEnumerable<Assembly> GetAppAssemblies()
            => this.GetAppRuntime()?.GetAppAssemblies() ?? Enumerable.Empty<Assembly>();

        /// <summary>
        /// Gets the application service infos in this collection.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the application service infos in this
        /// collection.
        /// </returns>
        public IEnumerable<ContractDeclaration> GetAppServiceContracts(IContext? context = null)
        {
            // Lite injector does not need to add to ambient services again its services
            // However, when the registration context and the candidate types are both null,
            // this is a message that ALL registration infos should be returned.
            if (context != null && ((bool?)this[InjectorExtensions.LiteInjectionKey] ?? false))
            {
                yield break;
            }

            // exclude the injector from the list as it is the responsibility
            // of each injector implementation to register itself in the DI container.
            foreach (ContractDeclaration s in this.registry.GetAppServiceContracts(context))
            {
                if (ReferenceEquals(s.ContractDeclarationType, typeof(IInjector)))
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
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.registry?.Dispose();
            }
        }
    }
}