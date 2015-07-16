// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Builder for ambient services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System.Diagnostics.Contracts;

    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Runtime;

    /// <summary>
    /// Builder for ambient services.
    /// </summary>
    public class AmbientServicesBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServicesBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public AmbientServicesBuilder(AmbientServices ambientServices)
        {
            Contract.Requires(ambientServices != null);

            this.AmbientServices = ambientServices;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServicesBuilder"/> class.
        /// </summary>
        public AmbientServicesBuilder()
            : this(AmbientServices.Instance)
        {
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public AmbientServices AmbientServices { get; private set; }

        /// <summary>
        /// Sets the log manager to the ambient services.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithLogManager(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.AmbientServices.LogManager = logManager;

            return this;
        }

        /// <summary>
        /// Sets the application configuration manager to the ambient services.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithConfigurationManager(IConfigurationManager configurationManager)
        {
            Contract.Requires(configurationManager != null);

            this.AmbientServices.ConfigurationManager = configurationManager;

            return this;
        }

        /// <summary>
        /// Sets the platform manager to the ambient services.
        /// </summary>
        /// <param name="platformManager">The platform manager.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithPlatformManager(IPlatformManager platformManager)
        {
            Contract.Requires(platformManager != null);

            this.AmbientServices.PlatformManager = platformManager;

            return this;
        }

        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <param name="compositionContainer">The composition container.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithCompositionContainer(ICompositionContext compositionContainer)
        {
            Contract.Requires(compositionContainer != null);

            this.AmbientServices.CompositionContainer = compositionContainer;

            return this;
        }
    }
}