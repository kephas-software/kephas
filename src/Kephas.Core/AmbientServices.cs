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
    using Kephas.Composition;
    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Lite;
    using Kephas.Composition.Lite.Conventions;
    using Kephas.Composition.Lite.Internal;
    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services.Composition;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Provides the global ambient services.
    /// </summary>
    /// <remarks>
    /// It is a recommended practice to not use global services, instead get the services
    /// using the composition (the classical example is the unit testing, where the classes
    /// should be sandboxed as much as possible). However, there may be cases when this cannot be avoided,
    /// such as static classes or classes which get instantiated outside of the developer's control
    /// (like in the case of the entities instatiated by the ORMs). Those are cases where the
    /// <see cref="AmbientServices"/> can be safely used.
    /// </remarks>
    [ExcludeFromComposition]
    public class AmbientServices : Expando, IAmbientServices, IAppServiceInfoProvider
    {
        private readonly IServiceRegistry registry = new ServiceRegistry();

        private readonly IResolverEngine resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        /// <param name="registerDefaultServices">Optional. True to register default services.</param>
        public AmbientServices(bool registerDefaultServices = true)
        {
            this.Register<IAmbientServices>(this)
                .Register<ICompositionContext>(this.AsCompositionContext());

            if (registerDefaultServices)
            {
                this
                    .Register<IConfigurationStore, DefaultConfigurationStore>()
                    .Register<ILogManager, NullLogManager>()
                    .Register<IAssemblyLoader, DefaultAssemblyLoader>()
                    .Register<ITypeLoader, DefaultTypeLoader>()
                    .Register<IAppRuntime, StaticAppRuntime>()

                    .RegisterMultiple<IConventionsRegistrar>(b => b.WithType<AppServiceInfoConventionsRegistrar>())

                    .RegisterMultiple<IAppServiceInfoProvider>(b => b.WithInstance(this))
                    .RegisterMultiple<IAppServiceInfoProvider>(b => b.WithType<AttributedAppServiceInfoProvider>());
            }

            this.registry
                .RegisterSource(new LazyServiceSource(this.registry))
                .RegisterSource(new LazyWithMetadataServiceSource(this.registry))
                .RegisterSource(new ExportFactoryServiceSource(this.registry))
                .RegisterSource(new ExportFactoryWithMetadataServiceSource(this.registry))
                .RegisterSource(new ListServiceSource(this.registry))
                .RegisterSource(new CollectionServiceSource(this.registry))
                .RegisterSource(new EnumerableServiceSource(this.registry));

            this.resolver = new ResolverEngine(this, this.registry);
        }

        /// <summary>
        /// Gets the configuration store.
        /// </summary>
        /// <value>
        /// The configuration store.
        /// </value>
        public IConfigurationStore ConfigurationStore => this.GetService<IConfigurationStore>();

        /// <summary>
        /// Gets the composition container.
        /// </summary>
        /// <value>
        /// The composition container.
        /// </value>
        public ICompositionContext CompositionContainer => this.GetService<ICompositionContext>();

        /// <summary>
        /// Gets the assembly loader.
        /// </summary>
        /// <value>
        /// The assembly loader.
        /// </value>
        public IAssemblyLoader AssemblyLoader => this.GetService<IAssemblyLoader>();

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime => this.GetService<IAppRuntime>();

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        public ILogManager LogManager => this.GetService<ILogManager>();

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
        public object GetService(Type serviceType)
        {
            return this.resolver.GetService(serviceType);
        }

        /// <summary>
        /// Gets the application service infos in this collection.
        /// </summary>
        /// <param name="candidateTypes">List of types of the candidates.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the application service infos in this
        /// collection.
        /// </returns>
        public IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(IList<Type> candidateTypes, ICompositionRegistrationContext registrationContext)
        {
            // Lite composition container does not need to add to ambient services again its services
            // However, when the registration context and the candidate types are both null,
            // this is a message that ALL registration infos should be returned.
            if (registrationContext != null && candidateTypes != null && ((bool?)this[LiteConventionsBuilder.LiteCompositionKey] ?? false))
            {
                return new (Type contractType, IAppServiceInfo appServiceInfo)[0];
            }

            // exclude the composition context from the list as it is the responsibility
            // of each composition context implementation to register itself in the DI container.
            return this.registry
                .Where(s => !ReferenceEquals(s.ContractType, typeof(ICompositionContext)))
                .SelectMany(s => this.ToAppServiceInfos(s).Select(si => (si.ContractType, si)))
                .ToList();
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