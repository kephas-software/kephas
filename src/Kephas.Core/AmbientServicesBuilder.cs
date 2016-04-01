// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Builder for ambient services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Configuration;
    using Kephas.Hosting;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Builder for ambient services.
    /// </summary>
    public class AmbientServicesBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServicesBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public AmbientServicesBuilder(AmbientServices ambientServices)
        {
            Contract.Requires(ambientServices != null);

            this.AmbientServices = ambientServices;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServicesBuilder"/> class.
        /// </summary>
        public AmbientServicesBuilder()
            : this((AmbientServices)AmbientServices.Instance)
        {
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public AmbientServices AmbientServices { get; private set; }

        /// <summary>
        /// Sets the log manager to the ambient services.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithLogManager(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.AmbientServices.LogManager = logManager;

            return this;
        }

        /// <summary>
        /// Sets the application configuration manager to the ambient services.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithConfigurationManager(IConfigurationManager configurationManager)
        {
            Contract.Requires(configurationManager != null);

            this.AmbientServices.ConfigurationManager = configurationManager;

            return this;
        }

        /// <summary>
        /// Sets the hosting environment to the ambient services.
        /// </summary>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithHostingEnvironment(IHostingEnvironment hostingEnvironment)
        {
            Contract.Requires(hostingEnvironment != null);

            this.AmbientServices.HostingEnvironment = hostingEnvironment;

            return this;
        }

        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <param name="compositionContainer">The composition container.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithCompositionContainer(ICompositionContext compositionContainer)
        {
            Contract.Requires(compositionContainer != null);

            this.AmbientServices.CompositionContainer = compositionContainer;

            return this;
        }

        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <typeparam name="TContainerBuilder">Type of the composition container builder.</typeparam>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <remarks>The container builder type must provide a constructor with one parameter of type <see cref="ICompositionContainerBuilderContext" />.</remarks>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithCompositionContainer<TContainerBuilder>(Action<TContainerBuilder> containerBuilderConfig = null)
            where TContainerBuilder : ICompositionContainerBuilder
        {
            var builderType = typeof(TContainerBuilder).AsDynamicTypeInfo();
            var context = new CompositionContainerBuilderContext(
                this.AmbientServices.LogManager,
                this.AmbientServices.ConfigurationManager,
                this.AmbientServices.HostingEnvironment);

            var containerBuilder = (TContainerBuilder)builderType.CreateInstance(new[] { context });

            containerBuilderConfig?.Invoke(containerBuilder);

            return this.WithCompositionContainer(containerBuilder.CreateContainer());
        }

        /// <summary>
        /// Sets asynchronously the composition container to the ambient services.
        /// </summary>
        /// <typeparam name="TContainerBuilder">Type of the composition container builder.</typeparam>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <remarks>The container builder type must provide a constructor with one parameter of type <see cref="ICompositionContainerBuilderContext" />.</remarks>
        /// <returns>A promise of the provided ambient services builder.</returns>
        public async Task<AmbientServicesBuilder> WithCompositionContainerAsync<TContainerBuilder>(Action<TContainerBuilder> containerBuilderConfig = null)
            where TContainerBuilder : ICompositionContainerBuilder
        {
            var builderType = typeof(TContainerBuilder).AsDynamicTypeInfo();
            var context = new CompositionContainerBuilderContext(
                this.AmbientServices.LogManager,
                this.AmbientServices.ConfigurationManager,
                this.AmbientServices.HostingEnvironment);

            var containerBuilder = (TContainerBuilder)builderType.CreateInstance(new[] { context });

            containerBuilderConfig?.Invoke(containerBuilder);

            return this.WithCompositionContainer(await containerBuilder.CreateContainerAsync().WithServerThreadingContext());
        }
    }
}