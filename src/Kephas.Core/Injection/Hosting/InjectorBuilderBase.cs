﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorBuilderBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Kephas.Collections;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Conventions;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Base class for injector builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    public abstract class InjectorBuilderBase<TBuilder> : Loggable, IInjectorBuilder
        where TBuilder : InjectorBuilderBase<TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectorBuilderBase{TBuilder}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected InjectorBuilderBase(IInjectionBuildContext context)
            : base((context ?? throw new ArgumentNullException(nameof(context))).AmbientServices.LogManager)
        {
            var ambientServices = context.AmbientServices;
            Requires.NotNull(ambientServices, nameof(ambientServices));

            this.BuildContext = context;

            this.Registry = new AppServiceInfoRegistry(
                () => context.Assemblies.Count == 0 ? this.GetDefaultAssemblies() : context.Assemblies);
        }

        /// <summary>
        /// Gets the <see cref="IAppServiceInfo"/> serviceRegistry.
        /// </summary>
        /// <value>
        /// The serviceRegistry.
        /// </value>
        protected internal AppServiceInfoRegistry Registry { get; }

        /// <summary>
        /// Gets the registration context.
        /// </summary>
        protected internal IInjectionBuildContext BuildContext { get; }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartBuilder"/> that must be used to specify the rule.</returns>
        public abstract IPartBuilder ForType(Type type);

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        public abstract IPartBuilder ForInstance(object instance);

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        public abstract IPartBuilder ForFactory(Type type, Func<IInjector, object> factory);

        /// <summary>
        /// Adds the assemblies containing the composition parts.
        /// </summary>
        /// <param name="assemblies">The composition assemblies.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assemblies are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithAssemblies(params Assembly[] assemblies)
            => this.WithAssemblies((IEnumerable<Assembly>)assemblies);

        /// <summary>
        /// Adds the assemblies containing the composition parts.
        /// </summary>
        /// <param name="assemblies">The composition assemblies.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assemblies are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithAssemblies(IEnumerable<Assembly> assemblies)
        {
            assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));

            this.BuildContext.Assemblies.AddRange(assemblies);

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the factory export.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="isSingleton">If set to <c>true</c>, the factory returns a shared component, otherwise an instance component.</param>
        /// <param name="allowMultiple">Indicates whether multiple registrations are allowed.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the factories are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithFactory<TContract>(Func<TContract> factory, bool isSingleton = false, bool allowMultiple = false)
        {
            Requires.NotNull(factory, nameof(factory));

            this.Registry.Add(new AppServiceInfo(
                typeof(TContract),
                ctx => factory(),
                isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient)
            {
                AllowMultiple = allowMultiple,
            });

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the registrations.
        /// </summary>
        /// <param name="registrations">A variable-length parameters list containing registrations.</param>
        /// <returns>
        /// A TBuilder.
        /// </returns>
        public virtual TBuilder WithRegistration(params IAppServiceInfo[] registrations)
        {
            registrations = registrations ?? throw new ArgumentNullException(nameof(registrations));

            registrations.ForEach(this.Registry.Add);

            return (TBuilder)this;
        }

        /// <summary>
        /// Creates the injector.
        /// </summary>
        /// <returns>The newly created injector.</returns>
        public virtual IInjector Build()
        {
            return Profiler.WithInfoStopwatch(
                () => this.RegisterConventions().CreateInjectorCore(),
                this.Logger).Value;
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfosProvider"/> objects.
        /// </returns>
        protected internal IList<IAppServiceInfosProvider> GetAppServiceInfosProviders()
        {
            return new List<IAppServiceInfosProvider>(this.BuildContext.AppServiceInfosProviders)
            {
                this.Registry,
            };
        }

        /// <summary>
        /// Adds the conventions from the provided types implementing
        /// <see cref="AppServiceInfoConventionsRegistrar" />.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        protected internal TBuilder RegisterConventions()
        {
            new AppServiceInfoConventionsRegistrar()
                .RegisterConventions(this, this.BuildContext, this.GetAppServiceInfosProviders());

            if (this.Logger.IsDebugEnabled())
            {
                this.Logger.Debug("Registering conventions from '{conventionsRegistrar}...", typeof(AppServiceInfoConventionsRegistrar));
            }

            return (TBuilder)this;
        }

        /// <summary>
        /// Gets the default assemblies if none provided in the context.
        /// </summary>
        /// <returns>A list of assemblies.</returns>
        protected virtual IList<Assembly> GetDefaultAssemblies()
        {
            var searchPattern = this.BuildContext.Settings.AssemblyFileNamePattern;

            this.Logger.Debug("{operation}. With assemblies matching pattern '{searchPattern}'.", nameof(this.GetDefaultAssemblies), searchPattern);

            return Profiler.WithDebugStopwatch(
                () =>
                {
                    var appAssemblies = this.WhereNotSystemAssemblies(this.BuildContext.AmbientServices.AppRuntime.GetAppAssemblies());

                    if (string.IsNullOrWhiteSpace(searchPattern))
                    {
                        return appAssemblies.ToList();
                    }

                    var regex = new Regex(searchPattern);
                    return appAssemblies.Where(a => regex.IsMatch(a.FullName!)).ToList();
                },
                this.Logger).Value;
        }

        /// <summary>
        /// Filters out the system assemblies from the provided assemblies.
        /// </summary>
        /// <param name="assemblies">The convention assemblies.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where not system assemblies in this
        /// collection.
        /// </returns>
        private IEnumerable<Assembly> WhereNotSystemAssemblies(IEnumerable<Assembly> assemblies)
        {
            return assemblies.Where(a => !a.IsSystemAssembly());
        }

        /// <summary>
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected abstract IInjector CreateInjectorCore();

        /// <summary>
        /// An application service information serviceRegistry.
        /// </summary>
        protected internal class AppServiceInfoRegistry : IAppServiceInfosProvider
        {
            private readonly Func<IEnumerable<Assembly>> getAssemblies;
            private readonly IList<IAppServiceInfo> appServiceInfos = new List<IAppServiceInfo>();

            /// <summary>
            /// Initializes a new instance of the <see cref="AppServiceInfoRegistry"/> class.
            /// </summary>
            /// <param name="getAssemblies">A function for retrieving the assemblies to scan for <see cref="IAppServiceInfosProvider"/> attributes.</param>
            public AppServiceInfoRegistry(Func<IEnumerable<Assembly>> getAssemblies)
            {
                this.getAssemblies = getAssemblies;
            }

            /// <summary>
            /// Adds an <see cref="IAppServiceInfo"/>.
            /// </summary>
            /// <param name="appServiceInfo">The Application service Information to add.</param>
            public void Add(IAppServiceInfo appServiceInfo)
            {
                this.appServiceInfos.Add(appServiceInfo);
            }

            /// <summary>
            /// Gets an enumeration of application service information objects.
            /// </summary>
            /// <param name="context">Optional. The context in which the service types are requested.</param>
            /// <returns>
            /// An enumeration of application service information objects and their associated contract type.
            /// </returns>
            public IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(dynamic? context = null)
            {
                return this.GetAppServices<IAppServiceInfosProvider>().SelectMany(p => p.GetAppServiceInfos())
                    .Union(this.appServiceInfos.Select(i => (i.ContractType!, i)));
            }

            /// <summary>
            /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
            /// </summary>
            /// <param name="context">Optional. The context in which the service types are requested.</param>
            /// <returns>
            /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
            /// </returns>
            public IEnumerable<(Type serviceType, Type contractDeclarationType)> GetAppServiceTypes(dynamic? context = null)
            {
                return this.GetAppServices<IAppServiceInfosProvider>().SelectMany(p => p.GetAppServiceTypes());
            }

            private IEnumerable<T> GetAppServices<T>()
            {
                var assemblies = this.getAssemblies();
                return assemblies
                    .SelectMany(a => a.GetCustomAttributes().OfType<T>())
                    .OrderBy(a => a is IHasProcessingPriority hasPriority ? hasPriority.ProcessingPriority : Priority.Normal);
            }
        }
    }
}