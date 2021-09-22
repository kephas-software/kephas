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

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Kephas.Injection.Lite;
    using Kephas.Injection.Lite.Conventions;
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
    public class AmbientServices : Expando, IAmbientServices, IAppServiceInfoProvider
    {
        private readonly IServiceRegistry registry = new ServiceRegistry();

        private readonly IResolverEngine resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        /// <param name="registerDefaultServices">Optional. True to register default services.</param>
        /// <param name="typeRegistry">Optional. The type registry.</param>
        public AmbientServices(bool registerDefaultServices = true, IRuntimeTypeRegistry? typeRegistry = null)
        {
            this.Register<IAmbientServices>(b => b.WithInstance(this).ExternallyOwned(true))
                .Register<IInjector>(b => b.WithInstance(this.AsInjector()).ExternallyOwned(true));

            typeRegistry ??= RuntimeTypeRegistry.Instance;
            this
                .Register<IRuntimeTypeRegistry>(typeRegistry);

            if (registerDefaultServices)
            {
                this
                    .Register<IConfigurationStore, DefaultConfigurationStore>()
                    .Register<ILogManager, NullLogManager>()
                    .Register<ITypeLoader, DefaultTypeLoader>()
                    .Register<ILicensingManager, NullLicensingManager>()

                    .RegisterMultiple<IConventionsRegistrar>(b => b.WithType<AppServiceInfoConventionsRegistrar>());
            }

            this.registry
                .RegisterSource(new LazyServiceSource(this.registry, typeRegistry))
                .RegisterSource(new LazyWithMetadataServiceSource(this.registry, typeRegistry))
                .RegisterSource(new ExportFactoryServiceSource(this.registry, typeRegistry))
                .RegisterSource(new ExportFactoryWithMetadataServiceSource(this.registry, typeRegistry))
                .RegisterSource(new ListServiceSource(this.registry, typeRegistry))
                .RegisterSource(new CollectionServiceSource(this.registry, typeRegistry))
                .RegisterSource(new EnumerableServiceSource(this.registry, typeRegistry));

            this.resolver = new ResolverEngine(this, this.registry);
        }

        /// <summary>
        /// Registers the provided service using a registration builder.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public virtual IAmbientServices Register(Type serviceType, Action<IServiceRegistrationBuilder> builder)
        {
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(builder, nameof(builder));

            var serviceBuilder = new ServiceRegistrationBuilder(this, serviceType);
            builder?.Invoke(serviceBuilder);
            this.registry.RegisterService(serviceBuilder.Build());

            return this;
        }

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        public bool IsRegistered(Type serviceType)
        {
            return serviceType != null && this.registry.IsRegistered(serviceType);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        /// <param name="serviceType">An object that specifies the type of service object to get. </param>
        public object? GetService(Type serviceType)
        {
            return this.resolver.GetService(serviceType);
        }

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
            // Lite composition container does not need to add to ambient services again its services
            // However, when the registration context and the candidate types are both null,
            // this is a message that ALL registration infos should be returned.
            if (context != null && ((bool?)this[LiteConventionsBuilder.LiteCompositionKey] ?? false))
            {
                return Array.Empty<(Type contractType, IAppServiceInfo appServiceInfo)>();
            }

            // exclude the composition context from the list as it is the responsibility
            // of each composition context implementation to register itself in the DI container.
            return this.registry
                .Where(s => !ReferenceEquals(s.ContractType, typeof(IInjector)))
                .SelectMany(s => this.ToAppServiceInfos(s).Select(si => (si.ContractType!, si)))
                .ToList();
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

        private IEnumerable<IAppServiceInfo> ToAppServiceInfos(IServiceInfo appServiceInfo)
        {
            switch (appServiceInfo)
            {
                case IEnumerable<IServiceInfo> multiServiceInfos:
                    foreach (ServiceInfo si in multiServiceInfos)
                    {
                        yield return si.ToAppServiceInfo(this);
                    }

                    break;
                case ServiceInfo serviceInfo:
                    yield return serviceInfo.ToAppServiceInfo(this);
                    break;
                default:
                    yield return appServiceInfo;
                    break;
            }
        }
    }
}