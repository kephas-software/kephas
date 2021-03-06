// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionContainerBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Builder for the MEF composition container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Composition.Hosting.Core;
    using System.Linq;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef.Conventions;
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Composition.Mef.Resources;
    using Kephas.Composition.Mef.ScopeFactory;
    using Kephas.Composition.Metadata;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Builder for the MEF composition container.
    /// </summary>
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    public class SystemCompositionContainerBuilder : CompositionContainerBuilderBase<SystemCompositionContainerBuilder>
    {
        /// <summary>
        /// The scope factories.
        /// </summary>
        private readonly ICollection<Type> scopeFactories = new HashSet<Type>();

        /// <summary>
        /// The container configuration.
        /// </summary>
        private ContainerConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionContainerBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SystemCompositionContainerBuilder(ICompositionRegistrationContext context)
            : base(context)
        {
            Requires.NotNull(context, nameof(context));
        }

        /// <summary>
        /// Gets the export providers.
        /// </summary>
        /// <value>
        /// The export providers.
        /// </value>
        protected IList<IExportProvider> ExportProviders { get; } = new List<IExportProvider>();

        /// <summary>
        /// Sets the composition conventions.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>This builder.</returns>
        public override SystemCompositionContainerBuilder WithConventions(IConventionsBuilder conventions)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var mefConventions = conventions as IMefConventionBuilderProvider;
            if (mefConventions == null)
            {
                throw new InvalidOperationException(string.Format(Strings.InvalidConventions, typeof(IMefConventionBuilderProvider).FullName));
            }

            return base.WithConventions(conventions);
        }

        /// <summary>
        /// Sets the container configuration.
        /// </summary>
        /// <param name="containerConfiguration">The container configuration.</param>
        /// <returns>This builder.</returns>
        public SystemCompositionContainerBuilder WithConfiguration(ContainerConfiguration containerConfiguration)
        {
            Requires.NotNull(containerConfiguration, nameof(containerConfiguration));

            this.configuration = containerConfiguration;

            return this;
        }

        /// <summary>
        /// Registers the scope factory <typeparamref name="TFactory"/>.
        /// </summary>
        /// <typeparam name="TFactory">Type of the factory.</typeparam>
        /// <returns>
        /// This builder.
        /// </returns>
        public SystemCompositionContainerBuilder WithScopeFactory<TFactory>()
            where TFactory : IMefScopeFactory
        {
            this.scopeFactories.Add(typeof(TFactory));

            return this;
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
        public virtual SystemCompositionContainerBuilder WithExportProvider(IExportProvider exportProvider)
        {
            Requires.NotNull(exportProvider, nameof(exportProvider));

            this.ExportProviders.Add(exportProvider);

            return this;
        }

        /// <summary>
        /// Registers the scope factory.
        /// </summary>
        /// <typeparam name="TFactory">Type of the factory.</typeparam>
        /// <param name="conventions">The conventions.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        protected SystemCompositionContainerBuilder RegisterScopeFactory<TFactory>(IConventionsBuilder conventions)
            where TFactory : IMefScopeFactory
        {
            return this.RegisterScopeFactory(conventions, typeof(TFactory));
        }

        /// <summary>
        /// Registers the scope factory.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="factoryType">Type of the factory.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        protected SystemCompositionContainerBuilder RegisterScopeFactory(IConventionsBuilder conventions, Type factoryType)
        {
            var mefConventions = ((IMefConventionBuilderProvider)conventions).GetConventionBuilder();

            var scopeName = factoryType.ExtractMetadataValue<CompositionScopeAttribute, string>(a => a.Value);
            mefConventions
                .ForType(factoryType)
                .Export(b => b.AsContractType<IMefScopeFactory>()
                              .AsContractName(scopeName))
                .Shared();

            return this;
        }

        /// <summary>
        /// Factory method for creating the MEF conventions builder.
        /// </summary>
        /// <returns>A newly created MEF conventions builder.</returns>
        protected override IConventionsBuilder CreateConventionsBuilder()
        {
            return new MefConventionsBuilder();
        }

        /// <summary>
        /// Creates a new composition container based on the provided conventions and assembly parts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="parts">The parts candidating for composition.</param>
        /// <returns>
        /// A new composition container.
        /// </returns>
        protected override ICompositionContext CreateContainerCore(IConventionsBuilder conventions, IEnumerable<Type> parts)
        {
            this.RegisterScopeFactoryConventions(conventions);

            var containerConfiguration = this.configuration ?? new ContainerConfiguration();
            var conventionBuilder = this.GetConventionBuilder(conventions);

            containerConfiguration
                .WithDefaultConventions(conventionBuilder)
                .WithParts(parts);

            this.RegisterScopeFactoryParts(containerConfiguration, parts);

            foreach (var provider in this.ExportProviders)
            {
                containerConfiguration.WithProvider((ExportDescriptorProvider)provider);
            }

            // add the default export descriptor providers.
            containerConfiguration
                .WithProvider(new ExportFactoryExportDescriptorProvider())
                .WithProvider(new ExportFactoryWithMetadataExportDescriptorProvider());

            foreach (var partBuilder in this.GetPartBuilders(conventions))
            {
                if (partBuilder.Instance != null)
                {
                    containerConfiguration.WithProvider(new FactoryExportDescriptorProvider(partBuilder.ContractType, () => partBuilder.Instance));
                }
                else
                {
                    containerConfiguration.WithProvider(new FactoryExportDescriptorProvider(partBuilder.ContractType, ctx => partBuilder.InstanceFactory(ctx), partBuilder.IsSingleton || partBuilder.IsScoped));
                }
            }

            return this.CreateCompositionContext(containerConfiguration);
        }

        /// <summary>
        /// Creates the composition context based on the provided container configuration.
        /// </summary>
        /// <param name="containerConfiguration">The container configuration.</param>
        /// <returns>
        /// The new composition context.
        /// </returns>
        protected virtual ICompositionContext CreateCompositionContext(ContainerConfiguration containerConfiguration)
        {
            return new SystemCompositionContainer(containerConfiguration);
        }

        /// <summary>
        /// Gets the convention builder out of the provided abstract conventions.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        protected virtual ConventionBuilder GetConventionBuilder(IConventionsBuilder conventions)
        {
            var mefConventions = ((IMefConventionBuilderProvider)conventions).GetConventionBuilder();
            return mefConventions;
        }

        /// <summary>
        /// Gets the part builders.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the part builders in this collection.
        /// </returns>
        protected virtual IEnumerable<MefPartBuilder> GetPartBuilders(IConventionsBuilder conventions)
        {
            return (conventions as MefConventionsBuilder)?.GetPartBuilders();
        }

        /// <summary>
        /// Registers the scope factories into the conventions.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        private void RegisterScopeFactoryConventions(IConventionsBuilder conventions)
        {
            this.scopeFactories.Add(typeof(DefaultMefScopeFactory));
            foreach (var scopeFactory in this.scopeFactories)
            {
                this.RegisterScopeFactory(conventions, scopeFactory);
            }
        }

        /// <summary>
        /// Registers the scope factory parts into the container configuration.
        /// </summary>
        /// <param name="containerConfiguration">The container configuration.</param>
        /// <param name="registeredParts">The registered parts.</param>
        private void RegisterScopeFactoryParts(ContainerConfiguration containerConfiguration, IEnumerable<Type> registeredParts)
        {
            if (this.scopeFactories.Count == 0)
            {
                return;
            }

            containerConfiguration.WithParts(this.scopeFactories.Where(f => !registeredParts.Contains(f)));
        }
    }
}