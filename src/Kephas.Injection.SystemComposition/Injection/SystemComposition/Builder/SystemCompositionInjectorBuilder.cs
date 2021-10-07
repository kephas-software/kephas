// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Builder for the MEF injector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Composition.Hosting.Core;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Injection.SystemComposition.ExportProviders;
    using Kephas.Injection.SystemComposition.ScopeFactory;

    /// <summary>
    /// Builder for the System.injector.
    /// </summary>
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    public class SystemCompositionInjectorBuilder : InjectorBuilderBase<SystemCompositionInjectorBuilder>
    {
        private readonly ICollection<Type> scopeFactories = new HashSet<Type>();
        private readonly ContainerConfiguration containerConfiguration;
        private readonly ConventionBuilder innerConventionBuilder;
        private readonly IList<ISystemCompositionRegistrationBuilder> partBuilders = new List<ISystemCompositionRegistrationBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionInjectorBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="containerConfiguration">Optional. The container configuration.</param>
        public SystemCompositionInjectorBuilder(IInjectionBuildContext context, ContainerConfiguration? containerConfiguration = null)
            : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
            this.containerConfiguration = containerConfiguration ?? new ContainerConfiguration();
            this.innerConventionBuilder = new ConventionBuilder();
        }

        /// <summary>
        /// Gets the export providers.
        /// </summary>
        /// <value>
        /// The export providers.
        /// </value>
        protected IList<IExportProvider> ExportProviders { get; } = new List<IExportProvider>();

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> that must be used to specify the rule.</returns>
        public override IRegistrationBuilder ForType(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            var partBuilder = new SystemCompositionTypeRegistrationBuilder(this.innerConventionBuilder.ForType(type), type);
            this.partBuilders.Add(partBuilder);
            return partBuilder;
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        public override IRegistrationBuilder ForInstance(object instance)
        {
            Requires.NotNull(instance, nameof(instance));

            var partBuilder = new SystemCompositionRegistrationBuilder(instance);
            partBuilder.Singleton().As(instance.GetType());
            this.partBuilders.Add(partBuilder);
            return partBuilder;
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        public override IRegistrationBuilder ForFactory(Type type, Func<IInjector, object> factory)
        {
            Requires.NotNull(factory, nameof(factory));

            var partBuilder = new SystemCompositionRegistrationBuilder(factory);
            partBuilder.As(type);
            this.partBuilders.Add(partBuilder);

            return partBuilder;
        }

        /// <summary>
        /// Registers the scope factory <typeparamref name="TFactory"/>.
        /// </summary>
        /// <typeparam name="TFactory">Type of the factory.</typeparam>
        /// <returns>
        /// This builder.
        /// </returns>
        public SystemCompositionInjectorBuilder WithScopeFactory<TFactory>()
            where TFactory : IScopeFactory
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
        public virtual SystemCompositionInjectorBuilder WithExportProvider(IExportProvider exportProvider)
        {
            Requires.NotNull(exportProvider, nameof(exportProvider));

            this.ExportProviders.Add(exportProvider);

            return this;
        }

        /// <summary>
        /// Registers the scope factory.
        /// </summary>
        /// <typeparam name="TFactory">Type of the factory.</typeparam>
        /// <returns>
        /// This builder.
        /// </returns>
        protected SystemCompositionInjectorBuilder RegisterScopeFactory<TFactory>()
            where TFactory : IScopeFactory
        {
            return this.RegisterScopeFactory(typeof(TFactory));
        }

        /// <summary>
        /// Registers the scope factory.
        /// </summary>
        /// <param name="factoryType">Type of the factory.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        protected SystemCompositionInjectorBuilder RegisterScopeFactory(Type factoryType)
        {
            var scopeName = factoryType.ExtractMetadataValue<InjectionScopeAttribute, string>(a => a.Value);
            this.innerConventionBuilder
                .ForType(factoryType)
                .Export(b => b.AsContractType<IScopeFactory>()
                              .AsContractName(scopeName))
                .Shared();

            return this;
        }

        /// <summary>
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected override IInjector CreateInjectorCore()
        {
            this.RegisterScopeFactoryConventions();

            foreach (var provider in this.ExportProviders)
            {
                this.containerConfiguration.WithProvider((ExportDescriptorProvider)provider);
            }

            // add the default export descriptor providers.
            this.containerConfiguration
                .WithProvider(new ExportFactoryExportDescriptorProvider())
                .WithProvider(new ExportFactoryWithMetadataExportDescriptorProvider());

            foreach (var partBuilder in this.GetPartBuilders())
            {
                partBuilder.Build(this.containerConfiguration);
            }

            this.containerConfiguration
                .WithDefaultConventions(this.innerConventionBuilder);

            return this.CreateCompositionContext(this.containerConfiguration);
        }

        /// <summary>
        /// Creates the injector based on the provided container containerConfiguration.
        /// </summary>
        /// <param name="containerConfiguration">The container configuration.</param>
        /// <returns>
        /// The new injector.
        /// </returns>
        protected virtual IInjector CreateCompositionContext(ContainerConfiguration containerConfiguration)
        {
            return new SystemCompositionInjector(containerConfiguration);
        }

        /// <summary>
        /// Gets the part builders.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the part builders in this collection.
        /// </returns>
        protected virtual IEnumerable<ISystemCompositionRegistrationBuilder> GetPartBuilders()
        {
            return this.partBuilders;
        }

        /// <summary>
        /// Registers the scope factories into the conventions.
        /// </summary>
        private void RegisterScopeFactoryConventions()
        {
            this.scopeFactories.Add(typeof(DefaultScopeFactory));
            foreach (var scopeFactory in this.scopeFactories)
            {
                this.RegisterScopeFactory(scopeFactory);
            }
        }
    }
}