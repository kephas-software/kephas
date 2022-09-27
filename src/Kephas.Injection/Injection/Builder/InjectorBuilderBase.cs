// --------------------------------------------------------------------------------------------------------------------
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
            this.lazyLogger = new Lazy<ILogger>(
                () => context.Logger ?? LoggingHelper.DefaultLogManager.GetLogger(this.GetType()));

            this.BuildContext = context;
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
        public abstract IRegistrationBuilder ForFactory(Type type, Func<IServiceProvider, object> factory);

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
        /// Creates the injector.
        /// </summary>
        /// <returns>The newly created injector.</returns>
        public virtual IServiceProvider Build()
        {
            return this.WithStopwatch(
                () => this.RegisterServices().CreateInjectorCore(),
                this.Logger,
                LogLevel.Info);
        }

        /// <summary>
        /// Registers the services from <see cref="IAppServiceInfo"/> collected from all providers.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        protected internal TBuilder RegisterServices()
        {
            foreach (var appServiceInfo in this.BuildContext.AmbientServices)
            {
                ((IInjectorBuilder)this).Register(appServiceInfo);
            }

            return (TBuilder)this;
        }

        /// <summary>
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected abstract IServiceProvider CreateInjectorCore();

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
            /// <returns>
            /// An enumeration of application service information objects and their associated contract type.
            /// </returns>
            public IEnumerable<ContractDeclaration> GetAppServiceContracts()
            {
                return this.appServiceInfos.Select(i => new ContractDeclaration(i.ContractType!, i));
            }

            /// <summary>
            /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
            /// </summary>
            /// <returns>
            /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
            /// </returns>
            public IEnumerable<ServiceDeclaration> GetAppServices()
            {
                yield break;
            }
        }
    }
}