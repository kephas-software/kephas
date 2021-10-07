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
        /// Registers the provided service.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builder">The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TContract>(this IAmbientServices ambientServices, Action<IServiceRegistrationBuilder> builder)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return ambientServices.Register(typeof(TContract), builder);
        }

        /// <summary>
        /// Registers the provided service, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builder">The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple<TContract>(this IAmbientServices ambientServices, Action<IServiceRegistrationBuilder> builder)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return ambientServices.Register(
                typeof(TContract),
                b =>
                    {
                        builder(b);
                        b.AllowMultiple();
                    });
        }

        /// <summary>
        /// Registers the provided service instance.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TContract>(this IAmbientServices ambientServices, TContract service)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            Requires.NotNull(service, nameof(service));

            return ambientServices.Register(typeof(TContract), b => b.ForInstance(service));
        }

        /// <summary>
        /// Registers the provided service instance, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple<TContract>(this IAmbientServices ambientServices, TContract service)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            Requires.NotNull(service, nameof(service));

            return ambientServices.Register(typeof(TContract), b => b.ForInstance(service).AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service with implementation type as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TService">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TContract, TService>(this IAmbientServices ambientServices)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.Register(
                typeof(TContract),
                b => b.ForType(typeof(TService))
                            .Singleton());
        }

        /// <summary>
        /// Registers the provided service with implementation type as singleton, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TService">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple<TContract, TService>(this IAmbientServices ambientServices)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.Register(
                typeof(TContract),
                b => b.ForType(typeof(TService))
                      .Singleton()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service with implementation type as transient.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TService">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransient<TContract, TService>(this IAmbientServices ambientServices)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.Register(
                typeof(TContract),
                b => b.ForType(typeof(TService))
                            .Transient());
        }

        /// <summary>
        /// Registers the provided service with implementation type as transient, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TService">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransientMultiple<TContract, TService>(this IAmbientServices ambientServices)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.Register(
                typeof(TContract),
                b => b.ForType(typeof(TService))
                      .Transient()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TContract>(
            this IAmbientServices ambientServices,
            Func<TContract> serviceFactory)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                typeof(TContract),
                b => b.ForFactory(ctx => serviceFactory())
                      .Singleton());
        }

        /// <summary>
        /// Registers the provided service as singleton factory, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple<TContract>(
            this IAmbientServices ambientServices,
            Func<TContract> serviceFactory)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                typeof(TContract),
                b => b.ForFactory(ctx => serviceFactory())
                      .Singleton()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service as transient factory.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransient<TContract>(
            this IAmbientServices ambientServices,
            Func<TContract> serviceFactory)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                typeof(TContract),
                b => b.ForFactory(ctx => serviceFactory())
                      .Transient());
        }

        /// <summary>
        /// Registers the provided service as transient factory, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TContract">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransientMultiple<TContract>(
            this IAmbientServices ambientServices,
            Func<TContract> serviceFactory)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                typeof(TContract),
                b => b.ForFactory(ctx => serviceFactory())
                      .Transient()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type contractType,
            Func<object> serviceFactory)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                contractType,
                b => b.ForFactory(ctx => serviceFactory())
                      .Singleton());
        }

        /// <summary>
        /// Registers the provided service as singleton factory, allowing also multiple registrations.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple(
            this IAmbientServices ambientServices,
            Type contractType,
            Func<object> serviceFactory)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                contractType,
                b => b.ForFactory(ctx => serviceFactory())
                      .Singleton()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service as transient factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransient(
            this IAmbientServices ambientServices,
            Type contractType,
            Func<object> serviceFactory)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                contractType,
                b => b.ForFactory(ctx => serviceFactory())
                      .Transient());
        }

        /// <summary>
        /// Registers the provided service as transient factory, allowing also multiple registrations.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransientMultiple(
            this IAmbientServices ambientServices,
            Type contractType,
            Func<object> serviceFactory)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                contractType,
                b => b.ForFactory(ctx => serviceFactory())
                      .Transient()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type contractType,
            object service)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            Requires.NotNull(service, nameof(service));

            return ambientServices.Register(contractType, b => b.ForInstance(service));
        }

        /// <summary>
        /// Registers the provided service, allowing also multiple registrations.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple(
            this IAmbientServices ambientServices,
            Type contractType,
            object service)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            Requires.NotNull(service, nameof(service));

            return ambientServices.Register(contractType, b => b.ForInstance(service).AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service as singleton.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type contractType,
            Type serviceImplementationType)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceImplementationType = serviceImplementationType ?? throw new ArgumentNullException(nameof(serviceImplementationType));

            ambientServices.Register(
                contractType,
                b => b.ForType(serviceImplementationType).Singleton());
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service as singleton, allowing also multiple registrations.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple(
            this IAmbientServices ambientServices,
            Type contractType,
            Type serviceImplementationType)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceImplementationType = serviceImplementationType ?? throw new ArgumentNullException(nameof(serviceImplementationType));

            ambientServices.Register(
                contractType,
                b => b.ForType(serviceImplementationType).Singleton().AllowMultiple());
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service as transient.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransient(
            this IAmbientServices ambientServices,
            Type contractType,
            Type serviceImplementationType)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceImplementationType = serviceImplementationType ?? throw new ArgumentNullException(nameof(serviceImplementationType));

            ambientServices.Register(
                contractType,
                b => b.ForType(serviceImplementationType).Transient());
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service as transient, allowing also multiple registrations.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransientMultiple(
            this IAmbientServices ambientServices,
            Type contractType,
            Type serviceImplementationType)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceImplementationType = serviceImplementationType ?? throw new ArgumentNullException(nameof(serviceImplementationType));

            ambientServices.Register(
                contractType,
                b => b.ForType(serviceImplementationType).Transient().AllowMultiple());
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
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <returns>
        /// A service object of type <paramref name="contractType"/>.
        /// </returns>
        public static object GetRequiredService(this IServiceProvider ambientServices, Type contractType)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));

            var service = ambientServices.GetService(contractType);
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
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// A service object of type <typeparamref name="TContract"/>.-or- <c>null</c> if there is no
        /// service object of type <typeparamref name="TContract"/>.
        /// </returns>
        public static TContract GetRequiredService<TContract>(this IServiceProvider ambientServices)
            where TContract : class
        {
            return (TContract)GetRequiredService(ambientServices, typeof(TContract));
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
            Requires.NotNull(encryptionService, nameof(encryptionService));

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

            ambientServices.Register<ILogManager>(b => b.ForInstance(logManager));

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
            Requires.NotNull(appRuntime, nameof(appRuntime));

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
            Requires.NotNull(injector, nameof(injector));

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