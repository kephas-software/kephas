// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerBuilderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for composition container builders.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Composition.Conventions;
    using Kephas.Configuration;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for composition container builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    public abstract class CompositionContainerBuilderBase<TBuilder> : ICompositionContainerBuilder
        where TBuilder : CompositionContainerBuilderBase<TBuilder>
    {
        /// <summary>
        /// The composition assembly file name pattern configuration key.
        /// </summary>
        public const string AssemblyNamePatternConfigurationKey = "composition:assemblyFileNamePattern";

        /// <summary>
        /// The context.
        /// </summary>
        private readonly IContext context;

        /// <summary>
        /// The composition assemblies.
        /// </summary>
        private HashSet<Assembly> compositionAssemblies;

        /// <summary>
        /// The convention assemblies.
        /// </summary>
        private HashSet<Assembly> conventionAssemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContainerBuilderBase{TBuilder}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Must register the ambient services in the composition context.")]
        protected CompositionContainerBuilderBase(IContext context)
        {
            Requires.NotNull(context, nameof(context));
            Contract.Requires(context.AmbientServices != null);

            this.context = context;
            this.ExportProviders = new List<IExportProvider>();

            this.LogManager = context.AmbientServices.GetService<ILogManager>();
            this.AssertRequiredService(this.LogManager);

            this.ConfigurationManager = context.AmbientServices.GetService<IConfigurationManager>();
            this.AssertRequiredService(this.ConfigurationManager);

            this.AppRuntime = context.AmbientServices.GetService<IAppRuntime>();
            this.AssertRequiredService(this.AppRuntime);

            this.TypeLoader = context.AmbientServices.GetService<ITypeLoader>();
            this.AssertRequiredService(this.TypeLoader);

            this.Logger = this.LogManager.GetLogger(this.GetType());

            this.WithServiceProviderExportProvider(context.AmbientServices);
        }

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        public ILogManager LogManager { get; }

        /// <summary>
        /// Gets the type loader.
        /// </summary>
        /// <value>
        /// The type loader.
        /// </value>
        public ITypeLoader TypeLoader { get; }

        /// <summary>
        /// Gets the configuration manager.
        /// </summary>
        /// <value>
        /// The configuration manager.
        /// </value>
        public IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime { get; }

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
        protected IConventionsBuilder ConventionsBuilder { get; private set; }

        /// <summary>
        /// Gets the composition parts.
        /// </summary>
        /// <value>
        /// The composition parts.
        /// </value>
        protected HashSet<Type> CompositionParts { get; private set; }

        /// <summary>
        /// Gets the export providers.
        /// </summary>
        /// <value>
        /// The export providers.
        /// </value>
        protected IList<IExportProvider> ExportProviders { get; }

        /// <summary>
        /// Adds the assemblies containing the conventions.
        /// </summary>
        /// <param name="assemblies">The convention assemblies.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assemblies are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithConventionAssemblies(IEnumerable<Assembly> assemblies)
        {
            Contract.Requires(assemblies != null);

            if (this.conventionAssemblies == null)
            {
                this.conventionAssemblies = new HashSet<Assembly>(assemblies);
            }
            else
            {
                this.conventionAssemblies.AddRange(assemblies);
            }

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the assembly containing the conventions.
        /// </summary>
        /// <param name="assembly">The convention assembly.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assembly is added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithConventionAssembly(Assembly assembly)
        {
            Requires.NotNull(assembly, nameof(assembly));

            return this.WithConventionAssemblies(new[] { assembly });
        }

        /// <summary>
        /// Sets the composition conventions.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>This builder.</returns>
        public virtual TBuilder WithConventions(IConventionsBuilder conventions)
        {
            Contract.Requires(conventions != null);

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
        public virtual TBuilder WithAssemblies(IEnumerable<Assembly> assemblies)
        {
            Contract.Requires(assemblies != null);

            if (this.compositionAssemblies == null)
            {
                this.compositionAssemblies = new HashSet<Assembly>(assemblies);
            }
            else
            {
                this.compositionAssemblies.AddRange(assemblies);
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
        /// Adds the composition parts.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided parts are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithParts(IEnumerable<Type> parts)
        {
            Contract.Requires(parts != null);

            if (this.CompositionParts == null)
            {
                this.CompositionParts = new HashSet<Type>(parts);
            }
            else
            {
                this.CompositionParts.AddRange(parts);
            }

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the composition parts.
        /// </summary>
        /// <param name="part">The composition part.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided part is added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithPart(Type part)
        {
            Contract.Requires(part != null);

            return this.WithParts(new[] { part });
        }

        /// <summary>
        /// Adds the factory export provider.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="isShared">If set to <c>true</c>, the factory returns a shared component, otherwise an instance component.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the factories are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithFactoryExportProvider<TContract>(Func<TContract> factory, bool isShared = false)
        {
            Contract.Requires(factory != null);
            Contract.Ensures(Contract.Result<TBuilder>() != null);

            var exportProvider = this.CreateFactoryExportProvider(factory, isShared);
            this.ExportProviders.Add(exportProvider);

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds an export provider based on the given <see cref="IServiceProvider"/>.
        /// </summary>
        /// <remarks>
        /// Can be used multiple times, the factories are added to the existing ones.
        /// </remarks>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public virtual TBuilder WithServiceProviderExportProvider(IServiceProvider serviceProvider)
        {
            Contract.Requires(serviceProvider != null);
            Contract.Ensures(Contract.Result<TBuilder>() != null);

            var exportProvider = this.CreateServiceProviderExportProvider(serviceProvider);
            this.ExportProviders.Add(exportProvider);

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the export provider.
        /// </summary>
        /// <remarks>
        /// Can be used multiple times, the factories are added to the existing ones.
        /// </remarks>
        /// <param name="exportProvider">The export provider.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public virtual TBuilder WithExportProvider(IExportProvider exportProvider)
        {
            Contract.Requires(exportProvider != null);
            Contract.Ensures(Contract.Result<TBuilder>() != null);

            this.ExportProviders.Add(exportProvider);

            return (TBuilder)this;
        }

        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        public virtual ICompositionContext CreateContainer()
        {
            ICompositionContext container = null;
            Profiler.WithInfoStopwatch(
                () =>
                {
                    var assemblies = this.compositionAssemblies;
                    if (assemblies == null)
                    {
                        throw new InvalidOperationException(Strings.CreateContainerRequiresCompositionAssembliesSet);
                    }

                    container = this.CreateContainerWithConventions(assemblies);
                },
                this.Logger);

            return container;
        }

        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        public virtual async Task<ICompositionContext> CreateContainerAsync()
        {
            ICompositionContext container = null;
            await Profiler.WithInfoStopwatchAsync(
                async () =>
                    {
                        var assemblies = await this.GetCompositionAssembliesAsync().PreserveThreadContext();

                        container = this.CreateContainerWithConventions(assemblies);
                    },
                this.Logger);

            return container;
        }

        /// <summary>
        /// Creates a new factory export provider.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="isShared">If set to <c>true</c>, the factory returns a shared component, otherwise an instance component.</param>
        /// <returns>
        /// The export provider.
        /// </returns>
        protected abstract IExportProvider CreateFactoryExportProvider<TContract>(Func<TContract> factory, bool isShared = false);

        /// <summary>
        /// Creates a new export provider based on a <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>
        /// The export provider.
        /// </returns>
        protected abstract IExportProvider CreateServiceProviderExportProvider(IServiceProvider serviceProvider);

        /// <summary>
        /// Factory method for creating the conventions builder.
        /// </summary>
        /// <returns>A newly created conventions builder.</returns>
        protected abstract IConventionsBuilder CreateConventionsBuilder();

        /// <summary>
        /// Creates a new composition container based on the provided conventions and assembly parts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="parts">The parts candidating for composition.</param>
        /// <returns>
        /// A new composition container.
        /// </returns>
        protected abstract ICompositionContext CreateContainerCore(IConventionsBuilder conventions, IEnumerable<Type> parts);

        /// <summary>
        /// Gets the composition assemblies.
        /// </summary>
        /// <returns>An enumeration of assemblies used for composition.</returns>
        protected async Task<IEnumerable<Assembly>> GetCompositionAssembliesAsync()
        {
            return (IEnumerable<Assembly>)this.compositionAssemblies ?? await this.GetAssembliesAsync();
        }

        /// <summary>
        /// Gets the convention assemblies.
        /// </summary>
        /// <param name="fallbackAssemblies">The fallback assemblies, used if no convention assemblies are provided.</param>
        /// <returns>An enumeration of assemblies used for conventions.</returns>
        protected IEnumerable<Assembly> GetConventionAssemblies(IEnumerable<Assembly> fallbackAssemblies)
        {
            return this.conventionAssemblies ?? fallbackAssemblies;
        }

        /// <summary>
        /// Gets the convention builder.
        /// </summary>
        /// <param name="assemblies">The assemblies containing the conventions.</param>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        protected virtual IConventionsBuilder GetConventions(IEnumerable<Assembly> assemblies, IList<Type> parts)
        {
            var conventions = this.ConventionsBuilder ?? this.CreateConventionsBuilder();

            if (assemblies == null || !assemblies.Any())
            {
                return conventions;
            }

            var assemblyNames = string.Join(", ", assemblies.Select(a => a.GetName().Name));
            this.Logger.Debug($"{nameof(this.GetConventions)}. Convention assemblies: {assemblyNames}.");

            Profiler.WithInfoStopwatch(
                () =>
                {
                    conventions.RegisterConventionsFrom(assemblies, parts, this.context);
                },
                this.Logger);

            return conventions;
        }

        /// <summary>
        /// Asserts the the required service is not missing.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="service">The service.</param>
        protected void AssertRequiredService<TService>(TService service)
        {
            if (service == null)
            {
                throw new InvalidOperationException(string.Format(Strings.CompositionContainerBuilderBase_RequiredServiceMissing_Exception, typeof(TService).FullName));
            }
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>The assemblies.</returns>
        private async Task<IList<Assembly>> GetAssembliesAsync(string searchPattern = null)
        {
            searchPattern = searchPattern ?? (string)this.ConfigurationManager.GetSetting(AssemblyNamePatternConfigurationKey);

            this.Logger.Debug($"{nameof(this.GetAssembliesAsync)}. With assemblies matching pattern '{searchPattern}'.");

            IList<Assembly> assemblies = null;

            await Profiler.WithDebugStopwatchAsync(
                async () =>
                {
                    var appAssemblies = await this.AppRuntime.GetAppAssembliesAsync();
                    appAssemblies = this.WhereNotSystemAssemblies(appAssemblies);

                    if (string.IsNullOrWhiteSpace(searchPattern))
                    {
                        assemblies = appAssemblies.ToList();
                    }
                    else
                    {
                        var regex = new Regex(searchPattern);
                        assemblies = appAssemblies.Where(a => regex.IsMatch(a.FullName)).ToList();
                    }
                },
                this.Logger);

            return assemblies;
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
        /// Creates the container with the registered conventions.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>The composition container.</returns>
        private ICompositionContext CreateContainerWithConventions(IEnumerable<Assembly> assemblies)
        {
            var parts = this.GetCompositionParts(assemblies);
            var conventionAssemblies = this.GetConventionAssemblies(assemblies);
            var conventions = this.GetConventions(conventionAssemblies, parts);

            var container = this.CreateContainerCore(conventions, parts);
            return container;
        }

        /// <summary>
        /// Gets the composition parts.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>The composition parts.</returns>
        private IList<Type> GetCompositionParts(IEnumerable<Assembly> assemblies)
        {
            var parts = assemblies.SelectMany(a => this.TypeLoader.GetLoadableExportedTypes(a)).ToList();
            if (this.CompositionParts != null)
            {
                parts.AddRange(this.CompositionParts);
            }

            return parts;
        }
    }
}