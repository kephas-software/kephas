// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServices.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides the global ambient services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// Provides the global ambient services.
    /// </summary>
    /// <remarks>
    /// It is a recommended practice to not use global services, instead get the services
    /// using the composition (the classical example is the unit testing, where the classes 
    /// should be sandboxed as much as possible). However, there may be cases when this cannot be avoided,
    /// such as static classes or classes which get instantiated outside of the developer's control 
    /// (like in the case of the entities instatiated by the ORMs). Those are cases where the
    /// <see cref="AmbientServices"/> can be safely used.
    /// </remarks>
    public class AmbientServices : Expando
    {
        /// <summary>
        /// The composition container.
        /// </summary>
        private ICompositionContext compositionContainer;

        /// <summary>
        /// The logger factory.
        /// </summary>
        private ILogManager logManager;

        /// <summary>
        /// The hosting environment.
        /// </summary>
        private IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// The application configuration provider.
        /// </summary>
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Initializes static members of the <see cref="AmbientServices"/> class.
        /// </summary>
        static AmbientServices()
        {
            Instance = new AmbientServices();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        internal AmbientServices()
        {
            this.CompositionContainer = new NullCompositionContainer();
            this.LogManager = new NullLogManager();
            this.HostingEnvironment = new NullHostingEnvironment();
            this.ConfigurationManager = new NullConfigurationManager();
        }

        /// <summary>
        /// Gets the static instance of the ambient services.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static AmbientServices Instance { get; private set; }

        /// <summary>
        /// Gets or sets the composition container.
        /// </summary>
        /// <value>
        /// The composition container.
        /// </value>
        public ICompositionContext CompositionContainer
        {
            get
            {
                return this.compositionContainer;
            }
            set
            {
                Contract.Requires(value != null);

                this.compositionContainer = value;
            }
        }

        /// <summary>
        /// Gets or sets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        public IHostingEnvironment HostingEnvironment
        {
            get
            {
                return this.hostingEnvironment;
            }
            set
            {
                Contract.Requires(value != null);

                this.hostingEnvironment = value;
            }
        }

        /// <summary>
        /// Gets or sets the application configuration provider.
        /// </summary>
        /// <value>
        /// The application configuration provider.
        /// </value>
        public IConfigurationManager ConfigurationManager
        {
            get
            {
                return this.configurationManager;
            }
            set
            {
                Contract.Requires(value != null);

                this.configurationManager = value;
            }
        }

        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        /// <value>
        /// The logger factory.
        /// </value>
        public ILogManager LogManager
        {
            get
            {
                return this.logManager;
            }
            set
            {
                Contract.Requires(value != null);

                this.logManager = value;
            }
        }

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger for the provided name.</returns>
        public static ILogger GetLogger(string loggerName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(loggerName));

            return Instance.LogManager.GetLogger(loggerName);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger(Type type)
        {
            Contract.Requires(type != null);

            return Instance.LogManager.GetLogger(type);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type for which a logger should be created.</typeparam>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger<T> GetLogger<T>()
        {
            return new TypedLogger<T>(Instance.LogManager);
        }
    }
}