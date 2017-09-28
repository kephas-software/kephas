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
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
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
            Requires.NotNull(ambientServices, nameof(ambientServices));

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
        public AmbientServices AmbientServices { get; }

        /// <summary>
        /// Sets the log manager to the ambient services.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithLogManager(ILogManager logManager)
        {
            Requires.NotNull(logManager, nameof(logManager));

            this.AmbientServices.RegisterService(logManager);

            return this;
        }

        /// <summary>
        /// Sets the application configuration to the ambient services.
        /// </summary>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithAppConfiguration(IAppConfiguration appConfiguration)
        {
            Requires.NotNull(appConfiguration, nameof(appConfiguration));

            this.AmbientServices.RegisterService(appConfiguration);

            return this;
        }

        /// <summary>
        /// Sets the application runtime to the ambient services.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithAppRuntime(IAppRuntime appRuntime)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));

            this.AmbientServices.RegisterService(appRuntime);

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
            Requires.NotNull(compositionContainer, nameof(compositionContainer));

            this.AmbientServices.RegisterService(compositionContainer);

            return this;
        }

        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <typeparam name="TContainerBuilder">Type of the composition container builder.</typeparam>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <remarks>The container builder type must provide a constructor with one parameter of type <see cref="IContext" />.</remarks>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithCompositionContainer<TContainerBuilder>(Action<TContainerBuilder> containerBuilderConfig = null)
            where TContainerBuilder : ICompositionContainerBuilder
        {
            var builderType = typeof(TContainerBuilder).AsRuntimeTypeInfo();
            var context = new CompositionRegistrationContext(this.AmbientServices);

            var containerBuilder = (TContainerBuilder)builderType.CreateInstance(new[] { context });

            containerBuilderConfig?.Invoke(containerBuilder);

            return this.WithCompositionContainer(containerBuilder.CreateContainer());
        }

        /// <summary>
        /// Sets asynchronously the composition container to the ambient services.
        /// </summary>
        /// <typeparam name="TContainerBuilder">Type of the composition container builder.</typeparam>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <remarks>The container builder type must provide a constructor with one parameter of type <see cref="IContext" />.</remarks>
        /// <returns>A promise of the provided ambient services builder.</returns>
        public async Task<AmbientServicesBuilder> WithCompositionContainerAsync<TContainerBuilder>(Action<TContainerBuilder> containerBuilderConfig = null)
            where TContainerBuilder : ICompositionContainerBuilder
        {
            var builderType = typeof(TContainerBuilder).AsRuntimeTypeInfo();
            var context = new CompositionRegistrationContext(this.AmbientServices);

            var containerBuilder = (TContainerBuilder)builderType.CreateInstance(new[] { context });

            containerBuilderConfig?.Invoke(containerBuilder);

            return this.WithCompositionContainer(await containerBuilder.CreateContainerAsync().PreserveThreadContext());
        }
    }
}