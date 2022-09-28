// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Injection.Configuration;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A service helper.
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <remarks>
        /// If the service implements <see cref="IAsyncInitializable"/>, the <see cref="IAsyncInitializable.InitializeAsync(IContext, CancellationToken)"/> method is called and its result is returned.
        /// If the service implements <see cref="IInitializable"/>, the <see cref="IInitializable.Initialize(IContext)"/> method is called and a completed task is returned.
        /// Otherwise nothing happens.
        /// </remarks>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task InitializeAsync<TService>(TService service, IContext? context = null, CancellationToken cancellationToken = default)
            where TService : class
        {
            switch (service)
            {
                case IAsyncInitializable asyncService:
                    return asyncService.InitializeAsync(context, cancellationToken);
                case IInitializable syncService:
                    syncService.Initialize(context);
                    return Task.CompletedTask;
                default:
                    return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <remarks>
        /// If the service implements <see cref="IAsyncInitializable"/>, the <see cref="IAsyncInitializable.InitializeAsync(IContext, CancellationToken)"/> method is synchronously called.
        /// If the service implements <see cref="IInitializable"/>, the <see cref="IInitializable.Initialize(IContext)"/> method is called.
        /// Otherwise nothing happens.
        /// </remarks>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="context">Optional. The context.</param>
        public static void Initialize<TService>(TService service, IContext? context = null)
            where TService : class
        {
            switch (service)
            {
                case IAsyncInitializable asyncService:
                    asyncService.InitializeAsync(context).WaitNonLocking();
                    break;
                case IInitializable syncService:
                    syncService.Initialize(context);
                    break;
            }
        }

        /// <summary>
        /// Finalizes the service asynchronously.
        /// </summary>
        /// <remarks>
        /// If the service implements <see cref="IAsyncFinalizable"/>, the <see cref="IAsyncFinalizable.FinalizeAsync(IContext, CancellationToken)"/> method is called and its result is returned.
        /// If the service implements <see cref="IFinalizable"/>, the <see cref="IFinalizable.Finalize(IContext)"/> method is called and a completed task is returned.
        /// If the service implements <see cref="IDisposable"/>, the <see cref="IDisposable.Dispose"/> method is called and a completed task is returned.
        /// Otherwise nothing happens.
        /// </remarks>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static async Task FinalizeAsync<TService>(TService service, IContext? context = null, CancellationToken cancellationToken = default)
            where TService : class
        {
            switch (service)
            {
                case IAsyncFinalizable asyncService:
                    await asyncService.FinalizeAsync(context, cancellationToken).PreserveThreadContext();
                    return;
                case IFinalizable syncService:
                    syncService.Finalize(context);
                    return;
                case IAsyncDisposable asyncDisposableService:
                    await asyncDisposableService.DisposeAsync().PreserveThreadContext();
                    return;
                case IDisposable disposableService:
                    disposableService.Dispose();
                    return;
            }

        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <remarks>
        /// If the service implements <see cref="IAsyncFinalizable"/>, the <see cref="IAsyncFinalizable.FinalizeAsync(IContext, CancellationToken)"/> method is called synchronously.
        /// If the service implements <see cref="IFinalizable"/>, the <see cref="IFinalizable.Finalize(IContext)"/> method is called.
        /// If the service implements <see cref="IDisposable"/>, the <see cref="IDisposable.Dispose"/> method is called.
        /// Otherwise nothing happens.
        /// </remarks>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="context">Optional. The context.</param>
        public static void Finalize<TService>(TService service, IContext? context = null)
            where TService : class
        {
            switch (service)
            {
                case IAsyncFinalizable asyncService:
                    asyncService.FinalizeAsync(context).WaitNonLocking();
                    break;
                case IFinalizable syncService:
                    syncService.Finalize(context);
                    break;
                case IAsyncDisposable asyncDisposableService:
                    asyncDisposableService.DisposeAsync().WaitNonLocking();
                    return;
                case IDisposable disposableService:
                    disposableService.Dispose();
                    break;
            }
        }

        /// <summary>
        /// Collects the metadata from the service type attributes and generic type arguments.
        /// </summary>
        /// <typeparam name="TMetadata">The metadata type.</typeparam>
        /// <param name="metadata">The metadata dictionary.</param>
        /// <returns>The typed metadata.</returns>
        public static TMetadata GetServiceMetadata<TMetadata>(IDictionary<string, object?>? metadata)
        {
            var ctor = typeof(TMetadata).GetConstructor(new[] { typeof(IDictionary<string, object>) });
            if (ctor is not null)
            {
                return (TMetadata)ctor.Invoke(new object?[] { metadata });
            }

            ctor = typeof(TMetadata).GetConstructor(Type.EmptyTypes);
            if (ctor is not null)
            {
                var typedMetadata = ctor.Invoke(Array.Empty<object>());
                if (metadata is not null)
                {
                    var dynamicMetadata = typedMetadata.ToDynamic();
                    metadata.ForEach(kv => dynamicMetadata[kv.Key] = kv.Value);
                }

                return (TMetadata)typedMetadata;
            }

            throw new ServiceException($"Cannot instantiate metadata of type {typeof(TMetadata)}, neither a constructor accepting an IDictionary<string, object> parameter, nor a default constructor is provided.");
        }

        /// <summary>
        /// Collects the metadata from the service type attributes and generic type arguments.
        /// </summary>
        /// <typeparam name="TMetadata">The metadata type.</typeparam>
        /// <param name="serviceType">The service type.</param>
        /// <param name="contractDeclarationType">The contract declaration type.</param>
        /// <returns>The typed metadata.</returns>
        public static TMetadata GetServiceMetadata<TMetadata>(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractDeclarationType) =>
            GetServiceMetadata<TMetadata>(GetServiceMetadata(serviceType, contractDeclarationType));

        /// <summary>
        /// Collects the metadata from the service type attributes and generic type arguments.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="contractDeclarationType">The contract declaration type.</param>
        /// <returns>The metadata.</returns>
        public static IDictionary<string, object?> GetServiceMetadata(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractDeclarationType)
        {
            var metadata = new Dictionary<string, object?>();
            serviceType?.GetCustomAttributes()
                .OfType<IMetadataProvider>()
                .SelectMany(p => p.GetMetadata())
                .ForEach(m => metadata[m.name] = m.value);

            metadata.Add(nameof(AppServiceMetadata.ServiceType), serviceType);

            // add metadata from generic parameters
            if (contractDeclarationType.IsGenericType)
            {
                var metadataSourceGenericType = contractDeclarationType.IsConstructedGenericType
                    ? contractDeclarationType
                    : serviceType is null || serviceType.IsGenericType
                        ? null
                        : serviceType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == contractDeclarationType);
                if (metadataSourceGenericType != null)
                {
                    IMetadataProvider.GetGenericTypeMetadataProvider(metadataSourceGenericType)
                        .GetMetadata()
                        .ForEach(m => metadata[m.name] = m.value);
                }
            }

            return metadata;
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <param name="appAssemblies">The application assemblies.</param>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfosProvider"/> objects.
        /// </returns>
        public static IEnumerable<IAppServiceInfosProvider> GetAppServiceInfosProviders(IEnumerable<Assembly> appAssemblies)
        {
            appAssemblies = appAssemblies ?? throw new ArgumentNullException(nameof(appAssemblies));

            var providers = appAssemblies
                .SelectMany(a => a.GetCustomAttributes().OfType<IAppServiceInfosProvider>())
                .OrderBy(a => a is IHasProcessingPriority hasPriority ? hasPriority.ProcessingPriority : Priority.Normal);

            return providers;
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <param name="buildContext">The build context.</param>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfosProvider"/> objects.
        /// </returns>
        public static IList<IAppServiceInfosProvider> GetAppServiceInfosProviders(this IAppServiceCollectionBuildContext buildContext)
        {
            var assemblies = buildContext.GetDefaultAssemblies();

            if (buildContext.Logger.IsDebugEnabled())
            {
                try
                {
                    buildContext.Logger.Debug("Using application assemblies: {assemblies}.", assemblies.Select(a => $"{a?.GetName()?.Name}, {a?.GetName()?.Version}").ToList());
                }
                catch (Exception ex)
                {
                    buildContext.Logger.Debug(ex, "Error while logging application assemblies.");
                }
            }

            var providers = GetAppServiceInfosProviders(assemblies)
                .Union(buildContext.AppServiceInfosProviders)
                .ToList();

            return providers;
        }

        /// <summary>
        /// Gets the default assemblies if none provided in the context.
        /// </summary>
        /// <returns>A list of assemblies.</returns>
        private static IList<Assembly> GetDefaultAssemblies(this IAppServiceCollectionBuildContext buildContext)
        {
            var searchPattern = buildContext.Settings.AssemblyFileNamePattern;

            buildContext.Logger.Debug("{operation}. With assemblies matching pattern '{searchPattern}'.", nameof(GetDefaultAssemblies), searchPattern);

            return WithStopwatch(
                () =>
                {
                    var appAssemblies = buildContext.Assemblies.WhereNotSystemAssemblies();

                    if (string.IsNullOrWhiteSpace(searchPattern))
                    {
                        return appAssemblies.ToList();
                    }

                    var regex = new Regex(searchPattern);
                    return appAssemblies.Where(a => regex.IsMatch(a.FullName!)).ToList();
                },
                buildContext.Logger,
                LogLevel.Debug);
        }

        /// <summary>
        /// Filters out the system assemblies from the provided assemblies.
        /// </summary>
        /// <param name="assemblies">The convention assemblies.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where not system assemblies in this
        /// collection.
        /// </returns>
        private static IEnumerable<Assembly> WhereNotSystemAssemblies(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.Where(a => !a.IsSystemAssembly());
        }

        /// <summary>
        /// Executes the action with a stopwatch, optionally logging the elapsed time at the indicated
        /// log level.
        /// </summary>
        /// <typeparam name="T">The operation return type.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="logger">Optional. The logger.</param>
        /// <param name="logLevel">Optional. The log level.</param>
        /// <param name="memberName">Optional. Name of the member.</param>
        /// <returns>
        /// The elapsed time.
        /// </returns>
        private static T WithStopwatch<T>(
            Func<T> action,
            ILogger? logger = null,
            LogLevel logLevel = LogLevel.Debug,
            [CallerMemberName] string? memberName = null)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));

            logger?.Log(logLevel, "{operation}. Started at: {startedAt:s}.", memberName, DateTime.Now);
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                var value = action();
                logger?.Log(logLevel, "{operation}. Ended at: {endedAt:s}. Elapsed: {elapsed:c}.", memberName, DateTime.Now, stopwatch.Elapsed);
                return value;
            }
            catch (Exception ex)
            {
                logger?.Log(LogLevel.Error, ex, "{operation}. Failed at: {endedAt:s}. Elapsed: {elapsed:c}.", memberName, DateTime.Now, stopwatch.Elapsed);
                throw;
            }
        }
    }
}
