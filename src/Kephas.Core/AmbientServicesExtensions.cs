// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Cryptography;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Injection.Lite;
    using Kephas.Injection.Lite.Builder;
    using Kephas.Licensing;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Extension methods for <see cref="IAmbientServices"/>.
    /// </summary>
    public static class AmbientServicesExtensions
    {
        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        public static ILogger GetLogger(this IAmbientServices ambientServices, string loggerName)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            Requires.NotNullOrEmpty(loggerName, nameof(loggerName));

            return ambientServices.LogManager.GetLogger(loggerName);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger(this IAmbientServices ambientServices, Type type)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            type = type ?? throw new ArgumentNullException(nameof(type));

            return ambientServices.LogManager.GetLogger(type);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type for which a logger should be created.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger<T>(this IAmbientServices ambientServices)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.LogManager.GetLogger(typeof(T));
        }

        /// <summary>
        /// Gets the runtime type registry.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The runtime type registry.
        /// </returns>
        public static IRuntimeTypeRegistry GetTypeRegistry(this IAmbientServices ambientServices) =>
            (ambientServices ?? throw new ArgumentNullException(nameof(ambientServices))).GetRequiredService<IRuntimeTypeRegistry>();

        /// <summary>
        /// Configures the settings.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="optionsConfig">The options configuration.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Configure<TSettings>(this IAmbientServices ambientServices, Action<TSettings> optionsConfig)
            where TSettings : class, new()
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            ambientServices.GetRequiredService<IConfigurationStore>().Configure(optionsConfig);
            return ambientServices;
        }

        /// <summary>
        /// Sets the configuration store to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="configurationStore">The configuration store.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithConfigurationStore(this IAmbientServices ambientServices, IConfigurationStore configurationStore)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            Requires.NotNull(configurationStore, nameof(configurationStore));

            ambientServices.Register(configurationStore);

            return ambientServices;
        }

        /// <summary>
        /// Sets the log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="logManager">The log manager.</param>
        /// <param name="replaceDefault">Optional. True to replace the <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithLogManager(this IAmbientServices ambientServices, ILogManager logManager, bool replaceDefault = true)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));

            if (replaceDefault)
            {
                LoggingHelper.DefaultLogManager = logManager;
            }

            ambientServices.Register<ILogManager>(logManager);

            return ambientServices;
        }

        /// <summary>
        /// Sets the injector to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="injector">The injector.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithInjector(this IAmbientServices ambientServices, IInjector injector)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            injector = injector ?? throw new ArgumentNullException(nameof(injector));

            ambientServices.Register(injector);

            return ambientServices;
        }

        /// <summary>
        /// Sets the injector to the ambient services.
        /// </summary>
        /// <typeparam name="TInjectorBuilder">Type of the injector builder.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builderOptions">The injector builder configuration.</param>
        /// <remarks>The container builder type must provide a constructor with one parameter of type <see cref="IContext" />.</remarks>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithInjector<TInjectorBuilder>(this IAmbientServices ambientServices, Action<TInjectorBuilder>? builderOptions = null)
            where TInjectorBuilder : IInjectorBuilder
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            var context = new InjectionBuildContext(ambientServices);

            var containerBuilder = (TInjectorBuilder)Activator.CreateInstance(typeof(TInjectorBuilder), context);

            builderOptions?.Invoke(containerBuilder);

            return ambientServices.WithInjector(containerBuilder.Build());
        }

        /// <summary>
        /// Builds the injector using Lite and adds it to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builderOptions">Optional. The injector builder configuration.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices BuildWithLite(this IAmbientServices ambientServices, Action<LiteInjectorBuilder>? builderOptions = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            var injectorBuilder = new LiteInjectorBuilder(new InjectionBuildContext(ambientServices));

            builderOptions?.Invoke(injectorBuilder);

            var container = injectorBuilder.Build();
            return ambientServices.WithInjector(container);
        }
    }
}