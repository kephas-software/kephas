// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorBuilderBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Configuration;
    using Kephas.Injection.Conventions;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Base class for injector builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    public abstract class InjectorBuilderBase<TBuilder> : IInjectorBuilder
        where TBuilder : InjectorBuilderBase<TBuilder>
    {
        private readonly InjectionSettings settings = new ();
        private HashSet<Assembly>? injectionAssemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectorBuilderBase{TBuilder}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Must register the ambient services in the injector.")]
        protected InjectorBuilderBase(IInjectionBuildContext context)
        {
            Requires.NotNull(context, nameof(context));
            var ambientServices = context.AmbientServices;
            Requires.NotNull(ambientServices, nameof(ambientServices));

            this.BuildContext = context;

            this.LogManager = ambientServices.LogManager;
            this.AssertRequiredService(this.LogManager);

            this.AppRuntime = ambientServices.AppRuntime;
            this.AssertRequiredService(this.AppRuntime);

            this.TypeLoader = ambientServices.TypeLoader;
            this.AssertRequiredService(this.TypeLoader);

            this.Logger = this.LogManager.GetLogger(this.GetType());

            this.Registry = new AppServiceInfoRegistry(this.GetAssemblies);

            context.AppServiceInfosProviders = context.AppServiceInfosProviders == null
                ? new List<IAppServiceInfosProvider> { this.Registry }
                : new List<IAppServiceInfosProvider>(context.AppServiceInfosProviders) { this.Registry };
        }

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        protected internal ILogManager LogManager { get; }

        /// <summary>
        /// Gets the type loader.
        /// </summary>
        /// <value>
        /// The type loader.
        /// </value>
        protected internal ITypeLoader TypeLoader { get; }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        protected internal IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the conventions builder.
        /// </summary>
        /// <value>
        /// The conventions builder.
        /// </value>
        protected IConventionsBuilder? ConventionsBuilder { get; private set; }

        /// <summary>
        /// Gets the <see cref="IAppServiceInfo"/> serviceRegistry.
        /// </summary>
        /// <value>
        /// The serviceRegistry.
        /// </value>
        protected AppServiceInfoRegistry Registry { get; }

        /// <summary>
        /// Gets the registration context.
        /// </summary>
        protected IInjectionBuildContext BuildContext { get; }

        /// <summary>
        /// Sets the composition conventions.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>This builder.</returns>
        public virtual TBuilder WithConventions(IConventionsBuilder conventions)
        {
            Requires.NotNull(conventions, nameof(conventions));

            this.ConventionsBuilder = conventions;

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the assemblies containing the composition parts.
        /// </summary>
        /// <param name="assemblies">The composition assemblies.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assemblies are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithAssemblies(params Assembly[] assemblies)
            => this.WithAssemblies((IEnumerable<Assembly>)assemblies);

        /// <summary>
        /// Adds the assemblies containing the composition parts.
        /// </summary>
        /// <param name="assemblies">The composition assemblies.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assemblies are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithAssemblies(IEnumerable<Assembly> assemblies)
        {
            Requires.NotNull(assemblies, nameof(assemblies));

            if (this.injectionAssemblies == null)
            {
                this.injectionAssemblies = new HashSet<Assembly>(assemblies);
            }
            else
            {
                this.injectionAssemblies.AddRange(assemblies);
            }

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the assembly containing the composition parts.
        /// </summary>
        /// <param name="assembly">The composition assembly.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assembly is added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithAssembly(Assembly assembly)
        {
            Requires.NotNull(assembly, nameof(assembly));

            return this.WithAssemblies(new[] { assembly });
        }

        /// <summary>
        /// Adds the factory export.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="isSingleton">If set to <c>true</c>, the factory returns a shared component, otherwise an instance component.</param>
        /// <param name="allowMultiple">Indicates whether multiple registrations are allowed.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the factories are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithFactory<TContract>(Func<TContract> factory, bool isSingleton = false, bool allowMultiple = false)
        {
            Requires.NotNull(factory, nameof(factory));

            this.Registry.Add(new AppServiceInfo(
                                    typeof(TContract),
                                    ctx => factory(),
                                    isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient)
                                {
                                    AllowMultiple = allowMultiple,
                                });

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the registrations.
        /// </summary>
        /// <param name="registrations">A variable-length parameters list containing registrations.</param>
        /// <returns>
        /// A TBuilder.
        /// </returns>
        public virtual TBuilder WithRegistration(params IAppServiceInfo[] registrations)
        {
            Requires.NotNull(registrations, nameof(registrations));

            registrations.ForEach(this.Registry.Add);

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the <see cref="IAppServiceInfosProvider"/>.
        /// </summary>
        /// <remarks>
        /// Can be used multiple times, the factories are added to the existing ones.
        /// </remarks>
        /// <param name="appServiceInfosProvider">The <see cref="IAppServiceInfosProvider"/>.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public virtual TBuilder WithAppServiceInfosProvider(IAppServiceInfosProvider appServiceInfosProvider)
        {
            Requires.NotNull(appServiceInfosProvider, nameof(appServiceInfosProvider));

            var registrars = this.BuildContext.AppServiceInfosProviders?.ToList() ?? new List<IAppServiceInfosProvider>();
            registrars.Add(appServiceInfosProvider);
            this.BuildContext.AppServiceInfosProviders = registrars;

            return (TBuilder)this;
        }

        /// <summary>
        /// Creates the injector.
        /// </summary>
        /// <returns>The newly created injector.</returns>
        public virtual IInjector Build()
        {
            IInjector? container = null;
            Profiler.WithInfoStopwatch(
                () =>
                {
                    container = this.CreateInjectorCore(this.GetConventions());
                },
                this.Logger);

            return container!;
        }

        /// <summary>
        /// Adds the composition parts.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided parts are added to the existing ones.
        /// </remarks>
        internal virtual TBuilder WithParts(IEnumerable<Type> parts)
        {
            Requires.NotNull(parts, nameof(parts));

            return this.WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts));
        }

        /// <summary>
        /// Factory method for creating the conventions builder.
        /// </summary>
        /// <returns>A newly created conventions builder.</returns>
        protected abstract IConventionsBuilder CreateConventionsBuilder();

        /// <summary>
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected abstract IInjector CreateInjectorCore(IConventionsBuilder conventions);

        /// <summary>
        /// Gets the convention builder.
        /// </summary>
        /// <returns>
        /// The convention builder.
        /// </returns>
        protected virtual IConventionsBuilder GetConventions()
        {
            var conventions = this.ConventionsBuilder ?? this.CreateConventionsBuilder();

            Profiler.WithInfoStopwatch(
                () =>
                {
                    conventions.RegisterConventions(this.BuildContext);
                },
                this.Logger);

            return conventions;
        }

        /// <summary>
        /// Asserts the required service is not missing.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="service">The service.</param>
        protected void AssertRequiredService<TService>(TService service)
        {
            if (service == null)
            {
                throw new InvalidOperationException(string.Format(Strings.InjectorBuilderBase_RequiredServiceMissing_Exception, typeof(TService).FullName));
            }
        }

        /// <summary>
        /// Gets the composition settings.
        /// </summary>
        /// <returns>
        /// The composition settings.
        /// </returns>
        protected virtual InjectionSettings GetSettings() => this.settings;

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>The assemblies.</returns>
        private IList<Assembly> GetAssemblies()
        {
            var searchPattern = this.GetSettings()?.AssemblyFileNamePattern;

            this.Logger.Debug("{operation}. With assemblies matching pattern '{searchPattern}'.", nameof(this.GetAssemblies), searchPattern);

            IList<Assembly>? assemblies = null;

            Profiler.WithDebugStopwatch(
                () =>
                {
                    var appAssemblies = this.injectionAssemblies
                                        ?? this.WhereNotSystemAssemblies(this.AppRuntime.GetAppAssemblies());

                    if (string.IsNullOrWhiteSpace(searchPattern))
                    {
                        assemblies = appAssemblies.ToList();
                    }
                    else
                    {
                        var regex = new Regex(searchPattern);
                        assemblies = appAssemblies.Where(a => regex.IsMatch(a.FullName!)).ToList();
                    }
                },
                this.Logger);

            return assemblies!;
        }

        /// <summary>
        /// Filters out the system assemblies from the provided assemblies.
        /// </summary>
        /// <param name="assemblies">The convention assemblies.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where not system assemblies in this
        /// collection.
        /// </returns>
        private IEnumerable<Assembly> WhereNotSystemAssemblies(IEnumerable<Assembly> assemblies)
        {
            return assemblies.Where(a => !a.IsSystemAssembly());
        }

        /// <summary>
        /// An application service information serviceRegistry.
        /// </summary>
        protected class AppServiceInfoRegistry : IAppServiceInfosProvider
        {
            private readonly Func<IEnumerable<Assembly>> getAssemblies;
            private readonly IList<IAppServiceInfo> appServiceInfos = new List<IAppServiceInfo>();

            /// <summary>
            /// Initializes a new instance of the <see cref="AppServiceInfoRegistry"/> class.
            /// </summary>
            /// <param name="getAssemblies">A function for retrieving the assemblies to scan for <see cref="IAppServiceInfosProvider"/> attributes.</param>
            public AppServiceInfoRegistry(Func<IEnumerable<Assembly>> getAssemblies)
            {
                this.getAssemblies = getAssemblies;
            }

            /// <summary>
            /// Adds an <see cref="IAppServiceInfo"/>.
            /// </summary>
            /// <param name="appServiceInfo">The Application service Information to add.</param>
            public void Add(IAppServiceInfo appServiceInfo)
            {
                this.appServiceInfos.Add(appServiceInfo);
            }

            /// <summary>
            /// Gets an enumeration of application service information objects.
            /// </summary>
            /// <param name="context">Optional. The context in which the service types are requested.</param>
            /// <returns>
            /// An enumeration of application service information objects and their associated contract type.
            /// </returns>
            public IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(dynamic? context = null)
            {
                return this.GetAppServices<IAppServiceInfosProvider>().SelectMany(p => p.GetAppServiceInfos())
                    .Union(this.appServiceInfos.Select(i => (i.ContractType!, i)));
            }

            /// <summary>
            /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
            /// </summary>
            /// <param name="context">Optional. The context in which the service types are requested.</param>
            /// <returns>
            /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
            /// </returns>
            public IEnumerable<(Type serviceType, Type contractDeclarationType)> GetAppServiceTypes(dynamic? context = null)
            {
                return this.GetAppServices<IAppServiceInfosProvider>().SelectMany(p => p.GetAppServiceTypes());
            }

            private IEnumerable<T> GetAppServices<T>()
            {
                return this.getAssemblies()
                    .SelectMany(a => a.GetCustomAttributes().OfType<T>())
                    .OrderBy(a => a is IHasProcessingPriority hasPriority ? hasPriority.ProcessingPriority : Priority.Normal);
            }
        }
    }
}