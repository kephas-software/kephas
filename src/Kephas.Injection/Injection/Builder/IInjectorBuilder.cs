// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IInjectorBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    using System;
    using System.Reflection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Resources;
    using Kephas.Resources;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Contract for injection builders.
    /// </summary>
    public interface IInjectorBuilder
    {
        /// <summary>
        /// Creates the injector.
        /// </summary>
        /// <returns>The newly created injector.</returns>
        IServiceProvider Build();

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> that must be used to specify the rule.</returns>
        IRegistrationBuilder ForType(Type type);

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        IRegistrationBuilder ForInstance(object instance);

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        IRegistrationBuilder ForFactory(Type type, Func<IServiceProvider, object> factory);

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        IRegistrationBuilder ForFactory<T>(Func<IServiceProvider, T> factory)
            => this.ForFactory(typeof(T), injector => factory(injector)!);

        /// <summary>
        /// Registers the <see cref="IAppServiceInfo"/> into the injector.
        /// </summary>
        /// <param name="appServiceInfo">The app service info.</param>
        void Register(IAppServiceInfo appServiceInfo)
        {
            var contractType = appServiceInfo.ContractType ?? throw new InjectionException(Strings.InjectorBuilder_RegisterService_InvalidContractType.FormatWith(appServiceInfo));
            var serviceBuilder = appServiceInfo.InstancingStrategy switch
            {
                Type type => this
                    .ForType(type)
                    .SelectConstructor(ctorInfos => this.TrySelectAppServiceConstructor(contractType, ctorInfos)),
                Func<IServiceProvider, object> factory => this
                    .ForFactory(contractType, factory),
                { } instance => this
                    .ForInstance(instance),
                _ => null,
            };

            if (serviceBuilder == null)
            {
                return;
            }

            serviceBuilder
                .As(contractType)
                .AllowMultiple(appServiceInfo.AllowMultiple);
            if (appServiceInfo.IsSingleton())
            {
                serviceBuilder.Singleton();
            }
            else if (appServiceInfo.IsScoped())
            {
                serviceBuilder.Scoped();
            }

            if (appServiceInfo.Metadata != null)
            {
                serviceBuilder.AddMetadata(appServiceInfo.Metadata);
            }

            if (appServiceInfo.IsExternallyOwned)
            {
                serviceBuilder.ExternallyOwned();
            }
        }

        private ConstructorInfo? TrySelectAppServiceConstructor(
            Type contractDeclarationType,
            IEnumerable<ConstructorInfo> constructors)
        {
            var constructorsList = constructors.Where(c => !c.IsStatic && c.IsPublic).ToList();

            // get the one constructor marked as InjectConstructor.
            var explicitlyMarkedConstructors = constructorsList
                .Where(c => c.GetCustomAttributes().OfType<IInjectConstructorAnnotation>().Any())
                .ToList();
            if (explicitlyMarkedConstructors.Count == 0)
            {
                // none marked explicitly, leave the decision up to the IoC implementation.
                return null;
            }

            if (explicitlyMarkedConstructors.Count > 1)
            {
                throw new InjectionException(
                    string.Format(
                        AbstractionStrings.AppServiceMultipleInjectConstructors,
                        typeof(InjectConstructorAttribute),
                        constructorsList[0].DeclaringType,
                        contractDeclarationType));
            }

            return explicitlyMarkedConstructors[0];
        }
    }
}