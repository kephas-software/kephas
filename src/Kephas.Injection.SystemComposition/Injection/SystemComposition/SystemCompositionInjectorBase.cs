﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionInjectorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF injector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Composition;
    using System.Linq;
    using System.Reflection;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.SystemComposition.Resources;
    using Kephas.Injection.SystemComposition.ScopeFactory;
    using Kephas.Reflection;

    /// <summary>
    /// A MEF injector.
    /// </summary>
    public abstract class SystemCompositionInjectorBase : IInjector, IServiceProvider
    {
        private static readonly MethodInfo ToEnumerableMethod = ReflectionHelper.GetGenericMethodOf(
            _ => SystemCompositionInjectorBase.ToEnumerable<int>(null));

        private static readonly MethodInfo ToListMethod = ReflectionHelper.GetGenericMethodOf(
            _ => SystemCompositionInjectorBase.ToList<int>(null));

        private static readonly ConcurrentDictionary<CompositionContext, IInjector> Map = new ();

        private CompositionContext? innerCompositionContext;

        /// <summary>
        /// Gets a value indicating whether this object is root.
        /// </summary>
        /// <value>
        /// True if this object is root, false if not.
        /// </value>
        protected virtual bool IsRoot => false;

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>An object implementing <paramref name="contractType"/>.</returns>
        public virtual object Resolve(Type contractType)
        {
            this.AssertNotDisposed();

            return this.TryResolveCollection(contractType)
                   ?? this.innerCompositionContext!.GetExport(contractType);
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>An enumeration of objects implementing <paramref name="contractType"/>.</returns>
        public virtual IEnumerable<object> ResolveMany(Type contractType)
        {
            this.AssertNotDisposed();

            var components = this.innerCompositionContext!.GetExports(contractType);
            return components;
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public virtual T Resolve<T>()
            where T : class
        {
            this.AssertNotDisposed();

            return this.TryResolveCollection(typeof(T)) as T
                   ?? this.innerCompositionContext!.GetExport<T>();
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An enumeration of objects implementing <typeparamref name="T" />.
        /// </returns>
        public virtual IEnumerable<T> ResolveMany<T>()
            where T : class
        {
            this.AssertNotDisposed();

            var components = this.innerCompositionContext!.GetExports<T>();
            return components;
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />, or <c>null</c> if a service with the provided contract was not found.
        /// </returns>
        public virtual object? TryResolve(Type contractType)
        {
            this.AssertNotDisposed();

            return this.TryResolveCollection(contractType)
                   ?? (this.innerCompositionContext!.TryGetExport(contractType, out var component) ? component : null);
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the provided contract was not found.
        /// </returns>
        public virtual T? TryResolve<T>()
            where T : class
        {
            this.AssertNotDisposed();

            return this.TryResolveCollection(typeof(T)) as T
                   ?? (this.innerCompositionContext!.TryGetExport(out T component) ? component : null);
        }

        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType" />.
        /// -or-
        /// <see langword="null" /> if there is no service object of type <paramref name="serviceType" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IServiceProvider.GetService?view=netstandard-2.1">`IServiceProvider.GetService` on docs.microsoft.com</a></footer>
        object? IServiceProvider.GetService(Type serviceType) => this.TryResolve(serviceType);

        /// <summary>
        /// Creates a new scoped injector.
        /// </summary>
        /// <returns>
        /// The new scoped injector.
        /// </returns>
        public virtual IInjector CreateScopedInjector()
        {
            var scopeProvider = this.innerCompositionContext!.GetExport<IScopeFactory>();

            var scopedContextExport = scopeProvider.CreateScopedContextExport();
            return GetOrAddCompositionContext(scopedContextExport);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Tries to get the injector wrapper for the provided injector.
        /// </summary>
        /// <param name="context">The inner container.</param>
        /// <param name="createNewIfMissing">True to create new if missing.</param>
        /// <returns>
        /// The injector wrapper.
        /// </returns>
        internal static IInjector? TryGetInjector(CompositionContext context, bool createNewIfMissing)
        {
            if (Map.TryGetValue(context, out var compositionContext))
            {
                return compositionContext;
            }

            return createNewIfMissing
                ? new ScopedSystemCompositionInjector(new System.Composition.Export<CompositionContext>(context, () => { }))
                : null;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.innerCompositionContext == null)
            {
                return;
            }

            Map.TryRemove(this.innerCompositionContext, out _);
            var disposableInnerContainer = this.innerCompositionContext as IDisposable;
            disposableInnerContainer?.Dispose();

            this.innerCompositionContext = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionInjectorBase"/> class.
        /// </summary>
        /// <param name="context">The inner container.</param>
        protected void Initialize(CompositionContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            this.innerCompositionContext = context;
            Map.TryAdd(context, this);

            if (this.IsRoot)
            {
                var rootContext = context.GetExport<CompositionContext>();
                Map.TryAdd(rootContext, this);
            }
        }

        /// <summary>
        /// Asserts that the container is not disposed.
        /// </summary>
        protected void AssertNotDisposed()
        {
            if (this.innerCompositionContext == null)
            {
                throw new ObjectDisposedException(Strings.SystemCompositionInjector_Disposed_Exception);
            }
        }

        private object? TryResolveCollection(Type contractType)
        {
            if (contractType.IsConstructedGenericOf(typeof(IEnumerable<>)) ||
                contractType.IsConstructedGenericOf(typeof(ICollection<>)) ||
                contractType.IsConstructedGenericOf(typeof(IList<>)) ||
                contractType.IsConstructedGenericOf(typeof(List<>)))
            {
                var exportType = contractType.TryGetEnumerableItemType();
                if (exportType != null)
                {
                    var exports = this.innerCompositionContext!.GetExports(exportType);
                    if (contractType.IsClass)
                    {
                        var toList = ToListMethod.MakeGenericMethod(exportType);
                        return toList.Call(null, exports);
                    }
                    else
                    {
                        var toEnumerable = ToEnumerableMethod.MakeGenericMethod(contractType);
                        return toEnumerable.Call(null, exports);
                    }
                }
            }

            return null;
        }

        private static IInjector GetOrAddCompositionContext(System.Composition.Export<CompositionContext> scopedContextExport)
        {
            return Map.GetOrAdd(scopedContextExport.Value, _ => new ScopedSystemCompositionInjector(scopedContextExport));
        }

        private static TEnumerable ToEnumerable<TEnumerable>(IEnumerable<object> exports)
        {
            return (TEnumerable)exports;
        }

        private static List<TItem> ToList<TItem>(IEnumerable<object> exports)
        {
            return exports.Cast<TItem>().ToList();
        }
    }
}
