// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerBuilderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for composition container builders.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Kephas.Composition.Conventions;
    using Kephas.Configuration;
    using Kephas.Diagnostics;
    using Kephas.Extensions;
    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Base class for composition container builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    public abstract class CompositionContainerBuilderBase<TBuilder>
        where TBuilder : CompositionContainerBuilderBase<TBuilder>
    {
        /// <summary>
        /// The composition assembly file name pattern configuration key.
        /// </summary>
        public const string AssemblyNamePatternConfigurationKey = "composition:assemblyFileNamePattern";

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
        /// <param name="logManager">The log manager.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="platformManager">The platform manager.</param>
        protected CompositionContainerBuilderBase(ILogManager logManager, IConfigurationManager configurationManager, IPlatformManager platformManager)
        {
            Contract.Requires(logManager != null);
            Contract.Requires(configurationManager != null);
            Contract.Requires(platformManager != null);

            this.ExportProviders = new Dictionary<Type, IExportProvider>();

            this.Logger = logManager.GetLogger(this.GetType());

            this.LogManager = logManager;
            this.ConfigurationManager = configurationManager;
            this.PlatformManager = platformManager;

            this.WithFactoryProvider(() => logManager, isShared: true)
                .WithFactoryProvider(() => configurationManager, isShared: true)
                .WithFactoryProvider(() => platformManager, isShared: true);
        }

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        public ILogManager LogManager { get; private set; }

        /// <summary>
        /// Gets the configuration manager.
        /// </summary>
        /// <value>
        /// The configuration manager.
        /// </value>
        public IConfigurationManager ConfigurationManager { get; private set; }

        /// <summary>
        /// Gets the runtime platform manager..
        /// </summary>
        /// <value>
        /// The runtime platform manager.
        /// </value>
        public IPlatformManager PlatformManager { get; private set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; private set; }

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
        protected IDictionary<Type, IExportProvider> ExportProviders { get; private set; }

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
            Contract.Requires(assembly != null);

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
            Contract.Requires(assembly != null);

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
        /// Adds the factory provider.
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
        public virtual TBuilder WithFactoryProvider<TContract>(Func<TContract> factory, bool isShared = false)
        {
            Contract.Requires(factory != null);
            Contract.Ensures(Contract.Result<TBuilder>() != null);

            var provider = this.CreateFactoryProvider(factory, isShared);
            this.ExportProviders[typeof(TContract)] = provider;

            return (TBuilder)this;
        }

        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        public virtual ICompositionContainer CreateContainer()
        {
            Contract.Ensures(Contract.Result<ICompositionContainer>() != null);

            ICompositionContainer container = null;
            this.Logger.SafeInfo("composition-container:create-container:begin");
            var elapsed = Profiler.WithStopwatch(
                () =>
                {
                    var assemblies = this.compositionAssemblies;
                    if (assemblies == null)
                    {
                        throw new InvalidOperationException(Strings.CreateContainerRequiresCompositionAssembliesSet);
                    }

                    container = this.CreateContainerWithAppServices(assemblies);
                });

            this.Logger.SafeInfo(string.Format("composition-container:create-container:end. Elapsed {0:c}.", elapsed));

            return container;
        }

        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        public virtual async Task<ICompositionContainer> CreateContainerAsync()
        {
            Contract.Ensures(Contract.Result<Task<ICompositionContainer>>() != null);

            ICompositionContainer container = null;
            this.Logger.SafeInfo("composition-container:create-container:begin");
            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                    {
                        var assemblies = await this.GetCompositionAssembliesAsync();

                        container = this.CreateContainerWithAppServices(assemblies);
                    });

            this.Logger.SafeInfo(string.Format("composition-container:create-container:end. Elapsed {0:c}.", elapsed));

            return container;
        }

        /// <summary>
        /// Registers the application services based on the declared contracts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="typeInfos">The type infos that contain the app services.</param>
        protected virtual void RegisterAppServices(IConventionsBuilder conventions, IEnumerable<TypeInfo> typeInfos)
        {
            // get all type infos from the composition assemblies
            var appServiceContractsInfos =
                typeInfos.ToDictionary(ti => ti, ti => ti.GetCustomAttribute<AppServiceContractAttribute>())
                    .Where(ta => ta.Value != null)
                    .ToList();

            foreach (var appServiceContractInfo in appServiceContractsInfos)
            {
                var serviceContract = appServiceContractInfo.Key;
                var serviceContractMetadata = appServiceContractInfo.Value;
                IPartConventionsBuilder partBuilder = null;
                if (serviceContractMetadata.AllowMultiple)
                {
                    // if the service contract metadata allows multiple service registrations
                    // then add just the conventions for the derived types.
                    partBuilder = conventions.ForTypesDerivedFrom(serviceContract.AsType());
                }
                else
                {
                    // if the service contract metadata does not allows multiple service registrations
                    // then ensure that only one service is registerd. If an export provider is provided
                    // for the service, then do nothing, otherwise try to identify the service to export 
                    // based on the OverridePriority attribute.
                    if (this.ExportProviders.ContainsKey(serviceContract.AsType()))
                    {
                        continue;
                    }

                    var parts =
                        typeInfos.Where(
                            ti =>
                            serviceContract.IsAssignableFrom(ti)
                            && ti.IsClass
                            && !ti.IsAbstract
                            && ti.GetCustomAttribute<ExcludeFromCompositionAttribute>() == null).ToList();
                    if (parts.Count == 1)
                    {
                        partBuilder = conventions.ForType(parts[0].AsType());
                    }
                    else if (parts.Count > 1)
                    {
                        var overrideChain = parts.ToDictionary(
                            ti => ti,
                            ti =>
                            ti.GetCustomAttribute<OverridePriorityAttribute>()
                            ?? new OverridePriorityAttribute(Priority.Normal)).OrderBy(item => item.Value.Value).ToList();

                        var selectedPart = overrideChain[0].Key;
                        if (overrideChain[0].Value.Value == overrideChain[1].Value.Value)
                        {
                            throw new InvalidOperationException(string.Format(Strings.AmbiguousOverrideForAppServiceContract, serviceContract, selectedPart, string.Join(", ", overrideChain.Select(item => item.Key.ToString() + ":" + item.Value.Value))));
                        }

                        partBuilder = conventions.ForType(selectedPart.AsType());
                    }
                }

                if (partBuilder != null)
                {
                    // TODO add support for metadata
                    partBuilder.Export(b => b.AsContractType(serviceContract.AsType()));

                    if (serviceContractMetadata.IsShared)
                    {
                        partBuilder.Shared();
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new factory provider.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="isShared">If set to <c>true</c>, the factory returns a shared component, otherwise an instance component.</param>
        /// <returns>
        /// The export provider.
        /// </returns>
        protected abstract IExportProvider CreateFactoryProvider<TContract>(Func<TContract> factory, bool isShared = false);

        /// <summary>
        /// Factory method for creating the conventions builder.
        /// </summary>
        /// <returns>A newly created conventions builder.</returns>
        protected abstract IConventionsBuilder CreateConventionsBuilder();

        /// <summary>
        /// Creates a new composition container based on the provided conventions and assembly parts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="assemblies">The composition assemblies.</param>
        /// <returns>
        /// A new composition container.
        /// </returns>
        protected abstract ICompositionContainer CreateContainerCore(IConventionsBuilder conventions, IEnumerable<Assembly> assemblies);

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
        /// <returns>
        /// The convention builder.
        /// </returns>
        protected virtual IConventionsBuilder GetConventions(IEnumerable<Assembly> assemblies)
        {
            var conventions = this.ConventionsBuilder ?? this.CreateConventionsBuilder();

            if (assemblies == null || !assemblies.Any())
            {
                return conventions;
            }

            this.Logger.SafeDebugFormat("composition-container:get-conventions:begin. Convention assemblies:\n'{0}'", string.Join(Environment.NewLine, assemblies.Select(a => a.GetName().Name)));

            var elapsed = Profiler.WithStopwatch(
                () =>
                {
                    conventions.WithConventionsFrom(assemblies);
                });

            this.Logger.SafeDebug(string.Format("composition-container:get-conventions:end. Elapsed {0:c}.", elapsed));

            return conventions;
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>The assemblies.</returns>
        private async Task<IList<Assembly>> GetAssembliesAsync(string searchPattern = null)
        {
            searchPattern = searchPattern ?? this.ConfigurationManager.GetSetting(AssemblyNamePatternConfigurationKey);

            this.Logger.SafeDebugFormat("composition-container:get-assemblies:begin. Assemblies matching '{0}'.", searchPattern);

            IList<Assembly> assemblies = null;

            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    var appAssemblies = await this.PlatformManager.GetAppAssembliesAsync();

                    if (string.IsNullOrWhiteSpace(searchPattern))
                    {
                        assemblies = appAssemblies.ToList();
                    }
                    else
                    {
                        var regex = new Regex(searchPattern);
                        assemblies = appAssemblies.Where(a => regex.IsMatch(a.FullName)).ToList();
                    }
                });

            this.Logger.SafeDebug(string.Format("composition-container:get-assemblies:end. Elapsed: {0:c}.", elapsed));

            return assemblies;
        }

        /// <summary>
        /// Creates the container with application services.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>The composition container.</returns>
        private ICompositionContainer CreateContainerWithAppServices(IEnumerable<Assembly> assemblies)
        {
            var conventionAssemblies = this.GetConventionAssemblies(assemblies);
            var conventions = this.GetConventions(conventionAssemblies);

            var typeInfos = assemblies.SelectMany(a => a.DefinedTypes).ToList();
            if (this.CompositionParts != null)
            {
                typeInfos.AddRange(this.CompositionParts.Select(t => t.GetTypeInfo()));
            }

            this.RegisterAppServices(conventions, typeInfos);
            var container = this.CreateContainerCore(conventions, assemblies);
            return container;
        }
    }
}