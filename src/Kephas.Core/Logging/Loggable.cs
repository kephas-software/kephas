// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Loggable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the loggable class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A loggable mixin class.
    /// </summary>
    public class Loggable : ILoggable
    {
        private static ILogManager logManager = new NullLogManager();

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        public Loggable()
            : this(DefaultLogManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public Loggable(IAmbientServices ambientServices)
            : this(ambientServices.LogManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public Loggable(ILogManager logManager)
        {
            if (logManager != null)
            {
                this.GetLogger = () => logManager.GetLogger(this.GetType());
            }
        }

        /// <summary>
        /// Gets or sets the default manager for log.
        /// </summary>
        /// <value>
        /// The default log manager.
        /// </value>
        public static ILogManager DefaultLogManager
        {
            get => logManager;
            set
            {
                Requires.NotNull(value, nameof(value));
                logManager = value;
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger Logger
        {
            get => this.logger ?? (this.logger = this.GetLogger?.Invoke() ?? LoggingExtensions.GetLogger(this, null));
            protected internal set => this.logger = value;
        }

        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        /// <returns>
        /// The logger factory.
        /// </returns>
        protected virtual Func<ILogger> GetLogger { get; set; }
    }
}