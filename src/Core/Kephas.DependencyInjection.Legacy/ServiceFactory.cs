// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.Reflection;
using Kephas.Services.Resources;
using Microsoft.Extensions.Logging;

namespace Kephas.Services;

/// <summary>
/// A context factory.
/// </summary>
[Priority(Priority.Low)]
public class ServiceFactory : IServiceFactory
{
    private const int AppServicesIndex = -1;
    private const int ServiceProviderIndex = -2;
    private const int LoggerFactoryIndex = -3;

    private readonly IServiceProvider serviceProvider;
    private readonly IAppServiceCollection appServices;
    private readonly ILoggerFactory loggerFactory;
    private readonly ConcurrentDictionary<Type, IList<(ConstructorInfo ctor, ParameterInfo[] paramInfos)>> typeCache = new ();
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Signature, Func<object?[], object>>> signatureCache = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The injector.</param>
    /// <param name="appServices">The application services.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ServiceFactory(IServiceProvider serviceProvider, IAppServiceCollection appServices, ILoggerFactory loggerFactory)
    {
        this.serviceProvider = serviceProvider;
        this.appServices = appServices;
        this.loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Creates an injectable instance.
    /// </summary>
    /// <param name="type">Type of the injectable.</param>
    /// <param name="args">A variable-length parameters list containing arguments.</param>
    /// <returns>
    /// The new injectable instance.
    /// </returns>
    public object Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, params object?[] args)
    {
        var injectableType = type ?? throw new ArgumentNullException(nameof(type));
        if (!injectableType.IsClass || injectableType.IsAbstract)
        {
            throw new ArgumentException(string.Format(Strings.InjectableFactory_Create_InjectableTypeMustBeInstantiable, injectableType));
        }

        // if (args.Any(a => a == null))
        // {
        //     throw new ArgumentException(Strings.ContextFactory_CreateContext_NonNullArguments.FormatWith(contextType));
        // }

        var signature = new Signature(args.Select(a => a?.GetType())!);
        var typeSignCache = signatureCache.GetOrAdd(injectableType, _ => new ConcurrentDictionary<Signature, Func<object?[], object>>());
        var creatorFunc = typeSignCache.GetOrAdd(signature, s => GetCreatorFunc(injectableType, s));
        return creatorFunc(args);
    }

    private Func<object?[], object> GetCreatorFunc(Type contextType, Signature signature)
    {
        var ctorInfos = typeCache.GetOrAdd(
            contextType,
            ct =>
            {
                var ctors = ct.GetConstructors().Where(c => c.IsPublic).OrderByDescending(c => c.GetParameters().Length).ToList();
                return ctors.Select(c => (c, c.GetParameters())).ToList();
            });
        foreach (var (ctor, paramInfos) in ctorInfos)
        {
            var (argIndexMap, argResolverMap) = GetSignatureMaps(signature, paramInfos);
            if (argIndexMap != null)
            {
                return args =>
                {
                    return ctor.Invoke(argIndexMap.Select(i => i >= 0 ? args[i] : argResolverMap![-i]()).ToArray());
                };
            }
        }

        throw new InvalidOperationException(string.Format(Strings.InjectableFactory_GetCreatorFunc_CannotFindMatchingConstructor, signature, contextType));
    }

    private (List<int>? argIndexMap, List<Func<object?>>? argResolverMap) GetSignatureMaps(Signature signature, ParameterInfo[] paramInfos)
    {
        var argIndexMap = new List<int>();
        var argResolverMap = new List<Func<object?>?>
        {
            null,
            () => appServices,
            () => serviceProvider,
            () => loggerFactory,
        };
        foreach (var paramInfo in paramInfos)
        {
            var paramType = paramInfo.ParameterType;
            var argIndex = GetArgIndex(signature, paramType);
            if (argIndex.HasValue)
            {
                argIndexMap.Add(argIndex.Value);
            }
            else
            {
                var appServiceInfo = appServices.FirstOrDefault(i => i.ServiceDeclarationType == paramType);
                if (appServiceInfo == null && paramType.IsGenericType)
                {
                    var genericDefParamType = paramType.GetGenericTypeDefinition();
                    appServiceInfo = appServices.FirstOrDefault(i => i.ServiceDeclarationType == genericDefParamType);
                }

                if (appServiceInfo != null && !appServiceInfo.AllowMultiple)
                {
                    argIndexMap.Add(-argResolverMap.Count);
                    argResolverMap.Add(() => serviceProvider.GetRequiredService(paramType));
                }
                else if (paramInfo.HasDefaultValue)
                {
                    argIndexMap.Add(-argResolverMap.Count);
                    argResolverMap.Add(() => paramInfo.DefaultValue);
                }
                else
                {
                    // there are params which cannot be resolved, break iteration.
                    return (null, null);
                }
            }
        }

        return (argIndexMap, argResolverMap)!;
    }

    private int? GetArgIndex(Signature signature, Type paramType)
    {
        if (paramType == typeof(IAppServiceCollection))
        {
            return AppServicesIndex;
        }

        if (paramType == typeof(IServiceProvider))
        {
            return ServiceProviderIndex;
        }

        if (paramType == typeof(ILoggerFactory))
        {
            return LoggerFactoryIndex;
        }

        for (var j = 0; j < signature.Count; j++)
        {
            var argType = signature[j];
            if (argType is not null && paramType.IsAssignableFrom(argType))
            {
                return j;
            }
        }

        return null;
    }
}
