// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Builder for ambient services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Lightweight;
    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Builder for ambient services.
    /// </summary>
    public class AmbientServicesBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServicesBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public AmbientServicesBuilder(IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            this.AmbientServices = ambientServices;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServicesBuilder"/> class.
        /// </summary>
        public AmbientServicesBuilder()
            : this(Kephas.AmbientServices.Instance)
        {
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Sets the configuration store to the ambient services.
        /// </summary>
        /// <param name="configurationStore">The configuration store.</param>
        /// <returns>
        /// This ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithConfigurationStore(IConfigurationStore configurationStore)
        {
            Requires.NotNull(configurationStore, nameof(configurationStore));

            this.AmbientServices.Register(configurationStore);

            return this;
        }

        /// <summary>
        /// Sets the log manager to the ambient services.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        /// <returns>
        /// This ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithLogManager(ILogManager logManager)
        {
            Requires.NotNull(logManager, nameof(logManager));

            this.AmbientServices.Register(logManager);

            return this;
        }

        /// <summary>
        /// Sets the application runtime to the ambient services.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// This ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithAppRuntime(IAppRuntime appRuntime)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));

            this.AmbientServices.Register(appRuntime);

            return this;
        }

        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <param name="compositionContainer">The composition container.</param>
        /// <returns>
        /// This ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithCompositionContainer(ICompositionContext compositionContainer)
        {
            Requires.NotNull(compositionContainer, nameof(compositionContainer));

            this.AmbientServices.Register(compositionContainer);

            return this;
        }

        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <typeparam name="TContainerBuilder">Type of the composition container builder.</typeparam>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <remarks>The container builder type must provide a constructor with one parameter of type <see cref="IContext" />.</remarks>
        /// <returns>
        /// This ambient services builder.
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
        /// Registers the provided service.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="builder">The service registration builder.</param>
        /// <returns>
        /// This ambient services builder.
        /// </returns>
        public AmbientServicesBuilder Register<TService>(Action<IServiceRegistrationBuilder> builder)
            where TService : class
        {
            Requires.NotNull(builder, nameof(builder));

            this.AmbientServices.Register(typeof(TService), builder);

            return this;
        }
    }
}