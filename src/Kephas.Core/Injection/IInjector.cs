// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Public interface for the injection context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.ExportFactoryImporters;
    using Kephas.Injection.Internal;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Public interface for the injection context.
    /// </summary>
    public interface IInjector : IDisposable
    {
        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceName">The service name.</param>
        /// <returns>An object implementing <paramref name="contractType"/>.</returns>
        object Resolve(Type contractType, string? serviceName = null);

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>An enumeration of objects implementing <paramref name="contractType"/>.</returns>
        IEnumerable<object> ResolveMany(Type contractType);

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        T Resolve<T>(string? serviceName = null)
            where T : class;

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An enumeration of objects implementing <typeparamref name="T" />.
        /// </returns>
        IEnumerable<T> ResolveMany<T>()
            where T : class;

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceName">The service name.</param>
        /// <returns>An object implementing <paramref name="contractType"/>, or <c>null</c> if a service with the provided contract was not found.</returns>
        object? TryResolve(Type contractType, string? serviceName = null);

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the provided contract was not found.
        /// </returns>
        T? TryResolve<T>(string? serviceName = null)
            where T : class;

        /// <summary>
        /// Creates a new scoped injector.
        /// </summary>
        /// <returns>
        /// The new scoped injector.
        /// </returns>
        IInjector CreateScopedInjector();
    }

    /// <summary>
    /// Extension methods for <see cref="IInjector"/>.
    /// </summary>
    public static class InjectorExtensions
    {
        /// <summary>
        /// Initializes static members of the <see cref="InjectorExtensions"/> class.
        /// </summary>
        static InjectorExtensions()
        {
            GetExportFactory1Method = ReflectionHelper.GetGenericMethodOf(_ => GetExportFactory<int>(null));
            GetExportFactory2Method = ReflectionHelper.GetGenericMethodOf(_ => GetExportFactory<int, int>(null));
            GetExportFactories1Method = ReflectionHelper.GetGenericMethodOf(_ => GetExportFactories<int>(null));
            GetExportFactories2Method = ReflectionHelper.GetGenericMethodOf(_ => GetExportFactories<int, int>(null));
        }

        /// <summary>
        /// Gets the <see cref="GetExportFactory{T}"/> method.
        /// </summary>
        /// <value>
        /// The <see cref="GetExportFactory{T}"/> method.
        /// </value>
        private static MethodInfo GetExportFactory1Method { get; }

        /// <summary>
        /// Gets the <see cref="GetExportFactory{T, TMetadata}"/> method.
        /// </summary>
        /// <value>
        /// The <see cref="GetExportFactory{T, TMetadata}"/> method.
        /// </value>
        private static MethodInfo GetExportFactory2Method { get; }

        /// <summary>
        /// Gets the <see cref="GetExportFactories{T}"/> method.
        /// </summary>
        /// <value>
        /// The <see cref="GetExportFactories{T}"/> method.
        /// </value>
        private static MethodInfo GetExportFactories1Method { get; }

        /// <summary>
        /// Gets the <see cref="GetExportFactories{T, TMetadata}"/> method.
        /// </summary>
        /// <value>
        /// The <see cref="GetExportFactories{T, TMetadata}"/> method.
        /// </value>
        private static MethodInfo GetExportFactories2Method { get; }

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="injector">The injection context to act on.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        [Pure]
        public static ILogger GetLogger(this IInjector injector, string loggerName)
        {
            Requires.NotNull(injector, nameof(injector));
            Requires.NotNullOrEmpty(loggerName, nameof(loggerName));

            return injector.Resolve<ILogManager>().GetLogger(loggerName);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="injector">The injection context to act on.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        [Pure]
        public static ILogger GetLogger(this IInjector injector, Type type)
        {
            Requires.NotNull(injector, nameof(injector));
            Requires.NotNull(type, nameof(type));

            return injector.Resolve<ILogManager>().GetLogger(type);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type for which a logger should be created.</typeparam>
        /// <param name="injector">The injection context to act on.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        [Pure]
        public static ILogger<T> GetLogger<T>(this IInjector injector)
        {
            Requires.NotNull(injector, nameof(injector));

            return new TypedLogger<T>(injector.Resolve<ILogManager>());
        }

        /// <summary>
        /// Converts a <see cref="IInjector"/> to a <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="injector">The injection context to act on.</param>
        /// <returns>
        /// The composition context as an <see cref="IServiceProvider"/>.
        /// </returns>
        public static IServiceProvider ToServiceProvider(this IInjector injector)
        {
            Requires.NotNull(injector, nameof(injector));

            return injector as IServiceProvider
                   ?? new ServiceProviderAdapter(injector);
        }

        /// <summary>
        /// Converts a <see cref="IServiceProvider"/> to a <see cref="IInjector"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider to act on.</param>
        /// <returns>
        /// The service provider as an <see cref="IInjector"/>.
        /// </returns>
        public static IInjector ToCompositionContext(this IServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));

            return serviceProvider as IInjector
                ?? new InjectorAdapter(serviceProvider);
        }

        /// <summary>
        /// Resolves the specified contract type as an export factory.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <param name="injector">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public static IExportFactory<T> GetExportFactory<T>(this IInjector injector)
        {
            Requires.NotNull(injector, nameof(injector));

            var importerType = typeof(IExportFactoryImporter<>).MakeGenericType(typeof(T));
            var importer = (IExportFactoryImporter)injector.Resolve(importerType);
            return (IExportFactory<T>)importer.ExportFactory;
        }

        /// <summary>
        /// Resolves the specified contract type as an export factory with metadata.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <typeparam name="TMetadata">The metadata type.</typeparam>
        /// <param name="injector">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public static IExportFactory<T, TMetadata> GetExportFactory<T, TMetadata>(this IInjector injector)
        {
            Requires.NotNull(injector, nameof(injector));

            var importerType = typeof(IExportFactoryImporter<,>).MakeGenericType(typeof(T), typeof(TMetadata));
            var importer = (IExportFactoryImporter)injector.Resolve(importerType);
            return (IExportFactory<T, TMetadata>)importer.ExportFactory;
        }

        /// <summary>
        /// Resolves the specified contract type as an enumeration of export factories.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <param name="injector">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public static IEnumerable<IExportFactory<T>> GetExportFactories<T>(this IInjector injector)
        {
            Requires.NotNull(injector, nameof(injector));

            var importerType = typeof(ICollectionExportFactoryImporter<>).MakeGenericType(typeof(T));
            var importer = (ICollectionExportFactoryImporter)injector.Resolve(importerType);
            return (IEnumerable<IExportFactory<T>>)importer.ExportFactories;
        }

        /// <summary>
        /// Resolves the specified contract type as an enumeration of export factories with metadata.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <typeparam name="TMetadata">The metadata type.</typeparam>
        /// <param name="injector">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public static IEnumerable<IExportFactory<T, TMetadata>> GetExportFactories<T, TMetadata>(this IInjector injector)
        {
            Requires.NotNull(injector, nameof(injector));

            var importerType = typeof(ICollectionExportFactoryImporter<,>).MakeGenericType(typeof(T), typeof(TMetadata));
            var importer = (ICollectionExportFactoryImporter)injector.Resolve(importerType);
            return (IEnumerable<IExportFactory<T, TMetadata>>)importer.ExportFactories;
        }

        /// <summary>
        /// Tries to resolve the specified contract type as an export factory.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <param name="injector">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c>.
        /// </returns>
        public static IExportFactory<T>? TryGetExportFactory<T>(this IInjector injector)
        {
            Requires.NotNull(injector, nameof(injector));

            var importerType = typeof(IExportFactoryImporter<>).MakeGenericType(typeof(T));
            var importer = (IExportFactoryImporter?)injector.TryResolve(importerType);
            return (IExportFactory<T>?)importer?.ExportFactory;
        }

        /// <summary>
        /// Tries to esolve the specified contract type as an export factory with metadata.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <typeparam name="TMetadata">The metadata type.</typeparam>
        /// <param name="injector">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c>.
        /// </returns>
        public static IExportFactory<T, TMetadata>? TryGetExportFactory<T, TMetadata>(this IInjector injector)
        {
            Requires.NotNull(injector, nameof(injector));

            var importerType = typeof(IExportFactoryImporter<,>).MakeGenericType(typeof(T), typeof(TMetadata));
            var importer = (IExportFactoryImporter?)injector.TryResolve(importerType);
            return (IExportFactory<T, TMetadata>?)importer?.ExportFactory;
        }

        /// <summary>
        /// Resolves the specified contract type as an export factory.
        /// </summary>
        /// <param name="injector">The compositionContext to act on.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// A export factory of an object implementing <paramref name="contractType"/>.
        /// </returns>
        public static object GetExportFactory(this IInjector injector, Type contractType)
        {
            Requires.NotNull(injector, nameof(injector));
            Requires.NotNull(contractType, nameof(contractType));

            var getExport = GetExportFactory1Method.MakeGenericMethod(contractType);
            return getExport.Call(null, injector);
        }

        /// <summary>
        /// Resolves the specified contract type as an export factory with metadata.
        /// </summary>
        /// <param name="injector">The compositionContext to act on.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="metadataType">Type of the metadata.</param>
        /// <returns>
        /// A export factory of an object implementing <paramref name="contractType"/> with <paramref name="metadataType"/> metadata.
        /// </returns>
        public static object GetExportFactory(this IInjector injector, Type contractType, Type metadataType)
        {
            Requires.NotNull(injector, nameof(injector));
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(metadataType, nameof(metadataType));

            var getExport = GetExportFactory2Method.MakeGenericMethod(contractType, metadataType);
            return getExport.Call(null, injector);
        }

        /// <summary>
        /// Resolves the specified contract type as an enumeration of export factories.
        /// </summary>
        /// <param name="injector">The compositionContext to act on.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An enumeration of export factories of an object implementing <paramref name="contractType"/>.
        /// </returns>
        public static IEnumerable GetExportFactories(this IInjector injector, Type contractType)
        {
            Requires.NotNull(injector, nameof(injector));
            Requires.NotNull(contractType, nameof(contractType));

            var getExport = GetExportFactories1Method.MakeGenericMethod(contractType);
            return (IEnumerable)getExport.Call(null, injector);
        }

        /// <summary>
        /// Resolves the specified contract type as an enumeration of export factories with metadata.
        /// </summary>
        /// <param name="injector">The compositionContext to act on.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="metadataType">Type of the metadata.</param>
        /// <returns>
        /// An enumeration of export factories of an object implementing <paramref name="contractType"/> with <paramref name="metadataType"/> metadata.
        /// </returns>
        public static IEnumerable GetExportFactories(this IInjector injector, Type contractType, Type metadataType)
        {
            Requires.NotNull(injector, nameof(injector));
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(metadataType, nameof(metadataType));

            var getExport = GetExportFactories2Method.MakeGenericMethod(contractType, metadataType);
            return (IEnumerable)getExport.Call(null, injector);
        }
    }
}