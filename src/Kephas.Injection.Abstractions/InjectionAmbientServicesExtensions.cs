// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;

    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Extensions for <see cref="IAmbientServices"/> for the injection subsystem.
    /// </summary>
    public static class InjectionAmbientServicesExtensions
    {
        internal const string AppServiceInfosKey = "__" + nameof(AppServiceInfosKey);

        /// <summary>
        /// Gets the registered application service contracts.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// An enumeration of key-value pairs, where the key is the <see cref="T:TypeInfo"/> and the
        /// value is the <see cref="IAppServiceInfo"/>.
        /// </returns>
        internal static IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(this IAmbientServices ambientServices)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices[AppServiceInfosKey] as IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)>
                   ?? Array.Empty<(Type contractType, IAppServiceInfo appServiceInfo)>();
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
        public static IAmbientServices Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAmbientServices ambientServices, TContract serviceInstance, Action<IRegistrationBuilder>? builder = null)
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
        public static IAmbientServices Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(this IAmbientServices ambientServices, Action<IRegistrationBuilder>? builder = null)
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
        public static IAmbientServices Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAmbientServices ambientServices, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType, Action<IRegistrationBuilder>? builder = null)
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
        public static IAmbientServices Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
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
        public static IAmbientServices Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
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
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
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
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
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
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
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
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
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
    }
}