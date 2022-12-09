﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorBuilderBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    using Kephas.Collections;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Base class for injector builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    public abstract class InjectorBuilderBase<TBuilder> : ILoggable, IInjectorBuilder, IHasInjectionBuildContext
        where TBuilder : InjectorBuilderBase<TBuilder>
    {
        private readonly Lazy<ILogger> lazyLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectorBuilderBase{TBuilder}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected InjectorBuilderBase(IInjectionBuildContext context)
        {
            var ambientServices = context.AmbientServices;
            ambientServices = ambientServices ?? throw new ArgumentException($"The {nameof(context)}.{nameof(context.AmbientServices)} is not set.", nameof(context));

            this.lazyLogger = new Lazy<ILogger>(
                () => (ambientServices.LogManager ?? LoggingHelper.DefaultLogManager).GetLogger(this.GetType()));

            this.BuildContext = context;

            this.Registry = new AppServiceInfoRegistry();
        }

        /// <summary>
        /// Gets the <see cref="IInjectionBuildContext"/>.
        /// </summary>
        public IInjectionBuildContext BuildContext { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger? Logger => this.lazyLogger.Value;

        /// <summary>
        /// Gets the <see cref="IAppServiceInfo"/> serviceRegistry.
        /// </summary>
        /// <value>
        /// The serviceRegistry.
        /// </value>
        protected internal AppServiceInfoRegistry Registry { get; }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> that must be used to specify the rule.</returns>
        public abstract IRegistrationBuilder ForType(Type type);

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        public abstract IRegistrationBuilder ForInstance(object instance);

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        public abstract IRegistrationBuilder ForFactory(Type type, Func<IInjector, object> factory);

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
            return this.WithStopwatch(
                () => this.RegisterServices().CreateInjectorCore(),
                this.Logger,
                LogLevel.Info);
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfosProvider"/> objects.
        /// </returns>
        protected internal IList<IAppServiceInfosProvider> GetAppServiceInfosProviders()
        {
            var assemblies = this.BuildContext.Assemblies.Count == 0
                ? this.GetDefaultAssemblies()
                : this.BuildContext.Assemblies;

            if (this.Logger.IsDebugEnabled())
            {
                var logAssemblies = assemblies;
                try
                {
                    this.Logger.Debug("Using application assemblies: {assemblies}.", logAssemblies.Select(a => $"{a?.GetName()?.Name}, {a?.GetName()?.Version}").ToList());
                }
                catch (Exception ex)
                {
                    this.Logger.Debug(ex, "Error while logging application assemblies.");
                }
            }

            var providers = ServiceHelper.GetAppServiceInfosProviders(assemblies)
                .Union(this.BuildContext.AppServiceInfosProviders)
                .Union(new[] { this.Registry })
                .ToList();

            return providers;
        }

        /// <summary>
        /// Registers the services from <see cref="IAppServiceInfo"/> collected from all providers.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        protected internal TBuilder RegisterServices()
        {
            var appServiceInfosProviders = this.GetAppServiceInfosProviders();
            if (this.Logger.IsDebugEnabled())
            {
                this.Logger.Debug("Registering conventions from providers '{appServiceInfosProviders}...", appServiceInfosProviders);
            }

            new AppServiceInfoInjectionRegistrar()
                .RegisterServices(this, this.BuildContext, appServiceInfosProviders);

            if (this.Logger.IsDebugEnabled())
            {
                this.Logger.Debug("Registering conventions from '{registrar}...", typeof(AppServiceInfoInjectionRegistrar));
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

            return this.WithStopwatch(
                () =>
                {
                    var rawAssemblies = this.BuildContext.GetAppAssemblies();
                    var appAssemblies = this.WhereNotSystemAssemblies(rawAssemblies);

                    if (string.IsNullOrWhiteSpace(searchPattern))
                    {
                        return appAssemblies.ToList();
                    }

                    var regex = new Regex(searchPattern);
                    return appAssemblies.Where(a => regex.IsMatch(a.FullName!)).ToList();
                },
                this.Logger,
                LogLevel.Debug);
        }

        /// <summary>
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected abstract IInjector CreateInjectorCore();

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
        protected virtual T WithStopwatch<T>(
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
        /// An application service information serviceRegistry.
        /// </summary>
        protected internal class AppServiceInfoRegistry : IAppServiceInfosProvider
        {
            private readonly IList<IAppServiceInfo> appServiceInfos = new List<IAppServiceInfo>();

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
            public IEnumerable<ContractDeclaration> GetAppServiceContracts(IContext? context = null)
            {
                return this.appServiceInfos.Select(i => new ContractDeclaration(i.ContractType!, i));
            }

            /// <summary>
            /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
            /// </summary>
            /// <param name="context">Optional. The context in which the contracts are requested.</param>
            /// <returns>
            /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
            /// </returns>
            public IEnumerable<ServiceDeclaration> GetAppServices(IContext? context = null)
            {
                yield break;
            }
        }
    }
}