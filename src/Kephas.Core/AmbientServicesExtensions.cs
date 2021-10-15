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
    using Kephas.Resources;
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

            ambientServices.ConfigurationStore.Configure(optionsConfig);
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service instance.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TContract>(this IAmbientServices ambientServices, TContract serviceInstance, Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            return ambientServices.RegisterService(typeof(TContract), serviceInstance, builder);
        }

        /// <summary>
        /// Registers the provided service with implementation type as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TService">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TContract, TService>(this IAmbientServices ambientServices, Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.RegisterService(
                typeof(TContract),
                typeof(TService),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service with implementation type as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TContract>(this IAmbientServices ambientServices, Type serviceType, Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            return ambientServices.RegisterService(
                typeof(TContract),
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TContract>(
            this IAmbientServices ambientServices,
            Func<TContract> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.RegisterService(
                typeof(TContract),
                (Func<IInjector, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TContract>(
            this IAmbientServices ambientServices,
            Func<IInjector, TContract> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.RegisterService(
                typeof(TContract),
                (Func<IInjector, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type contractType,
            Func<object> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.RegisterService(
                contractType,
                (Func<IInjector, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type contractType,
            Func<IInjector, object> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.RegisterService(
                contractType,
                (Func<IInjector, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service as singleton.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceType">The service implementation type.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type contractType,
            Type serviceType,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            ambientServices.RegisterService(
                contractType,
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service instance as singleton.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type contractType,
            object serviceInstance,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            ambientServices.RegisterService(
                contractType,
                serviceInstance,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
            return ambientServices;
        }

        /// <summary>
        /// Gets the service with the provided type.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// A service object of type <typeparamref name="TContract"/>.-or- <c>null</c> if there is no
        /// service object of type <typeparamref name="TContract"/>.
        /// </returns>
        public static TContract? GetService<TContract>(this IServiceProvider ambientServices)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return (TContract?)ambientServices.GetService(typeof(TContract));
        }

        /// <summary>
        /// Gets the service with the provided type.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <returns>
        /// A service object of type <paramref name="contractType"/>.
        /// </returns>
        public static object GetRequiredService(this IServiceProvider serviceProvider, Type contractType)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));

            var service = serviceProvider.GetService(contractType);
            if (service == null)
            {
                throw new InjectionException(
                    string.Format(
                        Strings.AmbientServices_RequiredServiceNotRegistered_Exception,
                        contractType));
            }

            return service;
        }

        /// <summary>
        /// Gets the service with the provided type.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>
        /// A service object of type <typeparamref name="TContract"/>.-or- <c>null</c> if there is no
        /// service object of type <typeparamref name="TContract"/>.
        /// </returns>
        public static TContract GetRequiredService<TContract>(this IServiceProvider serviceProvider)
            where TContract : class =>
            (TContract)GetRequiredService(serviceProvider, typeof(TContract));

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
        /// Sets the licensing manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="licensingManager">The licensing manager.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithLicensingManager(this IAmbientServices ambientServices, ILicensingManager licensingManager)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            Requires.NotNull(licensingManager, nameof(licensingManager));

            ambientServices.Register(licensingManager);

            return ambientServices;
        }

        /// <summary>
        /// Sets the default licensing manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="encryptionService">The encryption service.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithDefaultLicensingManager(this IAmbientServices ambientServices, IEncryptionService encryptionService)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));

            const string LicenseRepositoryKey = "__LicenseRepository";
            ambientServices.Register<ILicensingManager>(new DefaultLicensingManager(appid =>
                ((ambientServices[LicenseRepositoryKey] as ILicenseRepository)
                    ?? (ILicenseRepository)(ambientServices[LicenseRepositoryKey] = new LicenseRepository(ambientServices.AppRuntime, encryptionService)))
                        .GetLicenseData(appid)));

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
        /// Sets the application runtime to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithAppRuntime(this IAmbientServices ambientServices, IAppRuntime appRuntime)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));

            var existingAppRuntime = ambientServices.AppRuntime;
            if (existingAppRuntime != null && existingAppRuntime != appRuntime)
            {
                ServiceHelper.Finalize(existingAppRuntime);
            }

            if (existingAppRuntime != appRuntime)
            {
                ServiceHelper.Initialize(appRuntime);
                ambientServices.Register(appRuntime);
            }

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

            var builderType = ambientServices.TypeRegistry.GetTypeInfo(typeof(TInjectorBuilder));
            var context = new InjectionBuildContext(ambientServices);

            var containerBuilder = (TInjectorBuilder)builderType.CreateInstance(new[] { context });

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