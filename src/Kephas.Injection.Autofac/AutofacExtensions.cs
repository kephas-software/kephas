// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

using System;

using Autofac;
using Autofac.Builder;
using Autofac.Core.Resolving.Pipeline;
using Kephas.Injection.Autofac;
using Kephas.Injection.Autofac.Metadata;
using Kephas.Injection.Autofac.Resources;
using Kephas.Injection.Builder;
using Kephas.Logging;
using Kephas.Services;
using Kephas.Services.Reflection;

/// <summary>
/// Autofac related ambient services builder extensions.
/// </summary>
public static class AutofacExtensions
{
    /// <summary>
    /// Builds the injector with Autofac and adds it to the ambient services.
    /// </summary>
    /// <param name="ambientServices">The ambient services.</param>
    /// <param name="containerBuilder">The container builder.</param>
    /// <param name="preserveRegistrationOrder">Optional. Indicates whether to preserve the registration order. Relevant for integration with ASP.NET Core.</param>
    /// <param name="logger">Optional. The logger.</param>
    /// <returns>The provided ambient services.</returns>
    public static IServiceProvider BuildWithAutofac(this IAmbientServices ambientServices, ContainerBuilder? containerBuilder = null, bool preserveRegistrationOrder = true, ILogger? logger = null)
    {
        ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

        containerBuilder ??= new ContainerBuilder();
        containerBuilder.UseAmbientServices(ambientServices, preserveRegistrationOrder);

        return new AutofacServiceProvider(containerBuilder, logger);
    }

    /// <summary>
    /// Builds the injector with Autofac and adds it to the ambient services.
    /// </summary>
    /// <param name="containerBuilder">The container builder.</param>
    /// <param name="ambientServices">The ambient services.</param>
    /// <param name="preserveRegistrationOrder">Optional. Indicates whether to preserve the registration order. Relevant for integration with ASP.NET Core.</param>
    /// <param name="logger">Optional. The logger.</param>
    /// <returns>The provided ambient services.</returns>
    public static ContainerBuilder UseAmbientServices(this ContainerBuilder containerBuilder, IAmbientServices ambientServices, bool preserveRegistrationOrder = true, ILogger? logger = null)
    {
        containerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
        ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

        var buildContext = new InjectionBuildContext(ambientServices);
        buildContext.AddAppServices();

        foreach (var appServiceInfo in ambientServices)
        {
            containerBuilder.AddAppServiceInfo(appServiceInfo, preserveRegistrationOrder);
        }

        containerBuilder.RegisterSource(new ExportFactoryRegistrationSource());
        containerBuilder.RegisterSource(new ExportFactoryWithMetadataRegistrationSource());

        return containerBuilder;
    }

    /// <summary>
    /// Gets the lifetime scope from a component context.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="c">An IComponentContext to process.</param>
    /// <returns>
    /// The lifetime scope.
    /// </returns>
    public static ILifetimeScope GetLifetimeScope(this IComponentContext c)
    {
        return c switch
        {
            ILifetimeScope lifetimeScope => lifetimeScope,
            ResolveRequestContext resolveRequestContext => resolveRequestContext.ActivationScope,
            _ => throw new InvalidOperationException(
                string.Format(Strings.AutofacInjector_MismatchedLifetimeScope_Exception, c))
        };
    }

    /// <summary>
    /// Adds the application service to the <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="appServiceInfo">The <see cref="IAppServiceInfo"/>.</param>
    /// <param name="preserveRegistrationOrder">Optional. Indicates whether to preserve the registration order. Relevant for integration with Microsoft.Extensions.DependencyInjection.</param>
    /// <returns>The provided service collection.</returns>
    public static ContainerBuilder AddAppServiceInfo(
        this ContainerBuilder services,
        IAppServiceInfo appServiceInfo,
        bool preserveRegistrationOrder = true)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        var metadataType = appServiceInfo.MetadataType ?? typeof(AppServiceMetadata);
        var contractType = appServiceInfo.ContractType ?? appServiceInfo.InstanceType ?? throw new InvalidOperationException(Strings.AutofacExtensions_AddAppServiceInfo_ContractTypeNotSet.FormatWith(appServiceInfo));
        var instanceType = appServiceInfo.InstanceType;
        if (appServiceInfo.AsOpenGeneric && instanceType is null)
        {
            throw new InvalidOperationException(Strings.AutofacExtensions_AddAppServiceInfo_MustProvideServiceTypeForOpenGenerics.FormatWith(appServiceInfo));
        }

        return appServiceInfo switch
        {
            { InstanceType: not null and { IsGenericTypeDefinition: true } } =>
                services.AddGenericServiceType(contractType, instanceType!, appServiceInfo.Lifetime, appServiceInfo.Metadata, appServiceInfo.IsExternallyOwned),
            { InstanceType: not null } =>
                services.AddServiceType(contractType, instanceType!, appServiceInfo.Lifetime, appServiceInfo.Metadata, appServiceInfo.IsExternallyOwned, preserveRegistrationOrder),
            { InstanceFactory: not null } =>
                services.AddServiceFactory(contractType, appServiceInfo.InstanceFactory, appServiceInfo.Lifetime, appServiceInfo.Metadata, appServiceInfo.IsExternallyOwned, preserveRegistrationOrder),
            { InstancingStrategy: not null } =>
                services.AddServiceInstance(contractType, appServiceInfo.InstancingStrategy, appServiceInfo.Metadata, appServiceInfo.IsExternallyOwned, preserveRegistrationOrder),
            _ => services
        };
    }

    private static ContainerBuilder AddGenericServiceType(this ContainerBuilder services, Type contractType, Type serviceType, AppServiceLifetime lifetime, IDictionary<string, object?>? metadata, bool externallyOwned)
    {
        var builder = services.RegisterGeneric(serviceType)
            .As(contractType);

        builder = lifetime switch
        {
            AppServiceLifetime.Singleton => builder.SingleInstance(),
            AppServiceLifetime.Scoped => builder.InstancePerLifetimeScope(),
            AppServiceLifetime.Transient => builder.InstancePerDependency(),
            _ => builder,
        };

        if (metadata != null)
        {
            builder.WithMetadata(metadata);
        }

        if (externallyOwned)
        {
            builder.ExternallyOwned();
        }

        return services;
    }

    private static ContainerBuilder AddServiceType(this ContainerBuilder services, Type contractType, Type serviceType, AppServiceLifetime lifetime, IDictionary<string, object?>? metadata, bool externallyOwned, bool preserveRegistrationOrder)
    {
        var builder = RegistrationBuilder
            .ForType(serviceType)
            .As(contractType);

        builder = lifetime switch
        {
            AppServiceLifetime.Singleton => builder.SingleInstance(),
            AppServiceLifetime.Scoped => builder.InstancePerLifetimeScope(),
            AppServiceLifetime.Transient => builder.InstancePerDependency(),
            _ => builder,
        };

        if (metadata != null)
        {
            builder.WithMetadata(metadata);
        }

        if (preserveRegistrationOrder)
        {
            builder.PreserveExistingDefaults();
        }

        if (externallyOwned)
        {
            builder.ExternallyOwned();
        }

        var registration = builder.CreateRegistration();
        services.RegisterComponent(registration);

        return services;
    }

    private static ContainerBuilder AddServiceFactory(this ContainerBuilder services, Type contractType, Func<IServiceProvider, object> serviceFactory, AppServiceLifetime lifetime, IDictionary<string, object?>? metadata, bool externallyOwned, bool preserveRegistrationOrder)
    {
        var builder = RegistrationBuilder
            .ForDelegate(contractType, (ctx, _) => serviceFactory(ctx.Resolve<IServiceProvider>()))
            .As(contractType);

        switch (lifetime)
        {
            case AppServiceLifetime.Singleton:
                builder.SingleInstance();
                break;
            case AppServiceLifetime.Scoped:
                builder.InstancePerLifetimeScope();
                break;
        }

        if (metadata != null)
        {
            builder.WithMetadata(metadata);
        }

        if (preserveRegistrationOrder)
        {
            builder.PreserveExistingDefaults();
        }

        if (externallyOwned)
        {
            builder.ExternallyOwned();
        }

        var registration = builder.CreateRegistration();
        services.RegisterComponent(registration);

        return services;
    }

    private static ContainerBuilder AddServiceInstance(this ContainerBuilder services, Type contractType, object serviceInstance, IDictionary<string, object?>? metadata, bool externallyOwned, bool preserveRegistrationOrder)
    {
        var builder = services.RegisterInstance(serviceInstance)
            .As(contractType)
            .SingleInstance();

        if (metadata != null)
        {
            builder.WithMetadata(metadata);
        }

        if (preserveRegistrationOrder)
        {
            builder.PreserveExistingDefaults();
        }

        if (externallyOwned)
        {
            builder.ExternallyOwned();
        }

        return services;
    }
}