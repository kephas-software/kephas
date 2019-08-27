// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Public interface for the composition context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Composition.ExportFactoryImporters;
    using Kephas.Composition.Internal;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Public interface for the composition context.
    /// </summary>
    public interface ICompositionContext : IDisposable
    {
        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An object implementing <paramref name="contractType"/>.</returns>
        object GetExport(Type contractType, string contractName = null);

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An enumeration of objects implementing <paramref name="contractType"/>.</returns>
        IEnumerable<object> GetExports(Type contractType, string contractName = null);

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        T GetExport<T>(string contractName = null);

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An enumeration of objects implementing <typeparamref name="T" />.
        /// </returns>
        IEnumerable<T> GetExports<T>(string contractName = null);

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An object implementing <paramref name="contractType"/>, or <c>null</c> if a service with the provided contract was not found.</returns>
        object TryGetExport(Type contractType, string contractName = null);

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the provided contract was not found.
        /// </returns>
        T TryGetExport<T>(string contractName = null);

        /// <summary>
        /// Creates a new scoped composition context.
        /// </summary>
        /// <returns>
        /// The new scoped context.
        /// </returns>
        ICompositionContext CreateScopedContext();
    }

    /// <summary>
    /// Extension methods for <see cref="ICompositionContext"/>.
    /// </summary>
    public static class CompositionContextExtensions
    {
        /// <summary>
        /// Initializes static members of the <see cref="CompositionContextExtensions"/> class.
        /// </summary>
        static CompositionContextExtensions()
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
        /// <param name="compositionContext">The composition context to act on.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        [Pure]
        public static ILogger GetLogger(this ICompositionContext compositionContext, string loggerName)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNullOrEmpty(loggerName, nameof(loggerName));

            return compositionContext.GetExport<ILogManager>().GetLogger(loggerName);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="compositionContext">The composition context to act on.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        [Pure]
        public static ILogger GetLogger(this ICompositionContext compositionContext, Type type)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(type, nameof(type));

            return compositionContext.GetExport<ILogManager>().GetLogger(type);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type for which a logger should be created.</typeparam>
        /// <param name="compositionContext">The composition context to act on.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        [Pure]
        public static ILogger<T> GetLogger<T>(this ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            return new TypedLogger<T>(compositionContext.GetExport<ILogManager>());
        }

        /// <summary>
        /// Converts a <see cref="ICompositionContext"/> to a <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="compositionContext">The composition context to act on.</param>
        /// <returns>
        /// The composition context as an <see cref="IServiceProvider"/>.
        /// </returns>
        public static IServiceProvider ToServiceProvider(this ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            return compositionContext as IServiceProvider
                   ?? new ServiceProviderAdapter(compositionContext);
        }

        /// <summary>
        /// Converts a <see cref="IServiceProvider"/> to a <see cref="ICompositionContext"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider to act on.</param>
        /// <returns>
        /// The service provider as an <see cref="ICompositionContext"/>.
        /// </returns>
        public static ICompositionContext ToCompositionContext(this IServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));

            return serviceProvider as ICompositionContext
                ?? new CompositionContextAdapter(serviceProvider);
        }


        /// <summary>
        /// Resolves the specified contract type as an export factory.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <param name="compositionContext">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public static IExportFactory<T> GetExportFactory<T>(this ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            var importerType = typeof(IExportFactoryImporter<>).MakeGenericType(typeof(T));
            var importer = (IExportFactoryImporter)compositionContext.GetExport(importerType);
            return (IExportFactory<T>)importer.ExportFactory;
        }

        /// <summary>
        /// Resolves the specified contract type as an export factory with metadata.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <typeparam name="TMetadata">The metadata type.</typeparam>
        /// <param name="compositionContext">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public static IExportFactory<T, TMetadata> GetExportFactory<T, TMetadata>(this ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            var importerType = typeof(IExportFactoryImporter<,>).MakeGenericType(typeof(T), typeof(TMetadata));
            var importer = (IExportFactoryImporter)compositionContext.GetExport(importerType);
            return (IExportFactory<T, TMetadata>)importer.ExportFactory;
        }

        /// <summary>
        /// Resolves the specified contract type as an enumeration of export factories.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <param name="compositionContext">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public static IEnumerable<IExportFactory<T>> GetExportFactories<T>(this ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            var importerType = typeof(ICollectionExportFactoryImporter<>).MakeGenericType(typeof(T));
            var importer = (ICollectionExportFactoryImporter)compositionContext.GetExport(importerType);
            return (IEnumerable<IExportFactory<T>>)importer.ExportFactories;
        }

        /// <summary>
        /// Resolves the specified contract type as an enumeration of export factories with metadata.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <typeparam name="TMetadata">The metadata type.</typeparam>
        /// <param name="compositionContext">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public static IEnumerable<IExportFactory<T, TMetadata>> GetExportFactories<T, TMetadata>(this ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            var importerType = typeof(ICollectionExportFactoryImporter<,>).MakeGenericType(typeof(T), typeof(TMetadata));
            var importer = (ICollectionExportFactoryImporter)compositionContext.GetExport(importerType);
            return (IEnumerable<IExportFactory<T, TMetadata>>)importer.ExportFactories;
        }

        /// <summary>
        /// Tries to resolve the specified contract type as an export factory.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <param name="compositionContext">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c>.
        /// </returns>
        public static IExportFactory<T> TryGetExportFactory<T>(this ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            var importerType = typeof(IExportFactoryImporter<>).MakeGenericType(typeof(T));
            var importer = (IExportFactoryImporter)compositionContext.TryGetExport(importerType);
            return (IExportFactory<T>)importer?.ExportFactory;
        }

        /// <summary>
        /// Tries to esolve the specified contract type as an export factory with metadata.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <typeparam name="TMetadata">The metadata type.</typeparam>
        /// <param name="compositionContext">The compositionContext to act on.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c>.
        /// </returns>
        public static IExportFactory<T, TMetadata> TryGetExportFactory<T, TMetadata>(this ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            var importerType = typeof(IExportFactoryImporter<,>).MakeGenericType(typeof(T), typeof(TMetadata));
            var importer = (IExportFactoryImporter)compositionContext.TryGetExport(importerType);
            return (IExportFactory<T, TMetadata>)importer?.ExportFactory;
        }

        /// <summary>
        /// Resolves the specified contract type as an export factory.
        /// </summary>
        /// <param name="compositionContext">The compositionContext to act on.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// A export factory of an object implementing <paramref name="contractType"/>.
        /// </returns>
        public static object GetExportFactory(this ICompositionContext compositionContext, Type contractType)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(contractType, nameof(contractType));

            var getExport = GetExportFactory1Method.MakeGenericMethod(contractType);
            return getExport.Call(null, compositionContext);
        }

        /// <summary>
        /// Resolves the specified contract type as an export factory with metadata.
        /// </summary>
        /// <param name="compositionContext">The compositionContext to act on.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="metadataType">Type of the metadata.</param>
        /// <returns>
        /// A export factory of an object implementing <paramref name="contractType"/> with <paramref name="metadataType"/> metadata.
        /// </returns>
        public static object GetExportFactory(this ICompositionContext compositionContext, Type contractType, Type metadataType)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(metadataType, nameof(metadataType));

            var getExport = GetExportFactory2Method.MakeGenericMethod(contractType, metadataType);
            return getExport.Call(null, compositionContext);
        }

        /// <summary>
        /// Resolves the specified contract type as an enumeration of export factories.
        /// </summary>
        /// <param name="compositionContext">The compositionContext to act on.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An enumeration of export factories of an object implementing <paramref name="contractType"/>.
        /// </returns>
        public static IEnumerable GetExportFactories(this ICompositionContext compositionContext, Type contractType)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(contractType, nameof(contractType));

            var getExport = GetExportFactories1Method.MakeGenericMethod(contractType);
            return (IEnumerable)getExport.Call(null, compositionContext);
        }

        /// <summary>
        /// Resolves the specified contract type as an enumeration of export factories with metadata.
        /// </summary>
        /// <param name="compositionContext">The compositionContext to act on.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="metadataType">Type of the metadata.</param>
        /// <returns>
        /// An enumeration of export factories of an object implementing <paramref name="contractType"/> with <paramref name="metadataType"/> metadata.
        /// </returns>
        public static IEnumerable GetExportFactories(this ICompositionContext compositionContext, Type contractType, Type metadataType)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(metadataType, nameof(metadataType));

            var getExport = GetExportFactories2Method.MakeGenericMethod(contractType, metadataType);
            return (IEnumerable)getExport.Call(null, compositionContext);
        }
    }
}