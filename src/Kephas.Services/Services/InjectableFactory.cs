// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectableFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using Kephas;
    using Kephas.Services;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;

    /// <summary>
    /// A context factory.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class InjectableFactory : IInjectableFactory
    {
        private const int AmbientServicesIndex = -1;
        private const int InjectorIndex = -2;
        private const int LogManagerIndex = -3;

        private readonly IServiceProvider serviceProvider;
        private readonly IAmbientServices ambientServices;
        private readonly ConcurrentDictionary<Type, IList<(ConstructorInfo ctor, ParameterInfo[] paramInfos)>> typeCache = new ();

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Signature, Func<object?[], object>>> signatureCache = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="Kephas.Services.InjectableFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="logManager">Manager for log.</param>
        public InjectableFactory(IServiceProvider serviceProvider, IAmbientServices ambientServices, ILogManager logManager)
        {
            this.serviceProvider = serviceProvider;
            this.ambientServices = ambientServices;
            this.LogManager = logManager;
        }

        /// <summary>
        /// Gets the manager for log.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        public ILogManager LogManager { get; }

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
                throw new ArgumentException(AbstractionStrings.InjectableFactory_Create_InjectableTypeMustBeInstantiable.FormatWith(injectableType));
            }

            // if (args.Any(a => a == null))
            // {
            //     throw new ArgumentException(Strings.ContextFactory_CreateContext_NonNullArguments.FormatWith(contextType));
            // }

            var signature = new Signature(args.Select(a => a?.GetType())!);
            var typeSignCache = this.signatureCache.GetOrAdd(injectableType, _ => new ConcurrentDictionary<Signature, Func<object?[], object>>());
            var creatorFunc = typeSignCache.GetOrAdd(signature, _ => this.GetCreatorFunc(injectableType, signature));
            return creatorFunc(args);
        }

        private Func<object?[], object> GetCreatorFunc(Type contextType, Signature signature)
        {
            var ctorInfos = this.typeCache.GetOrAdd(
                contextType,
                _ =>
                {
                    var ctors = contextType.GetConstructors().Where(c => c.IsPublic).OrderByDescending(c => c.GetParameters().Length).ToList();
                    return ctors.Select(c => (c, c.GetParameters())).ToList();
                });
            foreach (var (ctor, paramInfos) in ctorInfos)
            {
                var (argIndexMap, argResolverMap) = this.GetSignatureMaps(signature, paramInfos);
                if (argIndexMap != null)
                {
                    return args =>
                    {
                        return ctor.Invoke(argIndexMap.Select(i => i >= 0 ? args[i] : argResolverMap![-i]()).ToArray());
                    };
                }
            }

            throw new InvalidOperationException(AbstractionStrings.InjectableFactory_GetCreatorFunc_CannotFindMatchingConstructor.FormatWith(signature, contextType));
        }

        private (List<int>? argIndexMap, List<Func<object?>>? argResolverMap) GetSignatureMaps(Signature signature, ParameterInfo[] paramInfos)
        {
            var argIndexMap = new List<int>();
            var argResolverMap = new List<Func<object?>?>
                {
                    null,
                    () => this.ambientServices,
                    () => this.serviceProvider,
                    () => this.LogManager,
                };
            foreach (var paramInfo in paramInfos)
            {
                var paramType = paramInfo.ParameterType;
                var argIndex = this.GetArgIndex(signature, paramType);
                if (argIndex.HasValue)
                {
                    argIndexMap.Add(argIndex.Value);
                }
                else
                {
                    var appServiceInfo = this.ambientServices.FirstOrDefault(i => i.ContractDeclarationType == paramType);
                    if (appServiceInfo == null && paramType.IsGenericType)
                    {
                        var genericDefParamType = paramType.GetGenericTypeDefinition();
                        appServiceInfo = this.ambientServices.FirstOrDefault(i => i.ContractDeclarationType == genericDefParamType);
                    }

                    if (appServiceInfo != null && !appServiceInfo.AllowMultiple)
                    {
                        argIndexMap.Add(-argResolverMap.Count);
                        argResolverMap.Add(() => this.serviceProvider.Resolve(paramType));
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
            if (paramType == typeof(IAmbientServices))
            {
                return AmbientServicesIndex;
            }

            if (paramType == typeof(IServiceProvider))
            {
                return InjectorIndex;
            }

            if (paramType == typeof(ILogManager))
            {
                return LogManagerIndex;
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
}
