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

    using Kephas.Services;
    using Kephas.Services;

    /// <summary>
    /// A loggable mixin class.
    /// </summary>
    public abstract class Loggable : ILoggable
    {
        private Lazy<ILogger> lazyLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        protected Loggable()
            : this(() => LoggingHelper.DefaultLogManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        protected Loggable(IServiceProvider serviceProvider)
            : this(() => serviceProvider?.GetRequiredService<ILogManager>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        protected Loggable(IInjectableFactory injectableFactory)
            : this(() => injectableFactory?.GetLogManager())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected Loggable(ILogger logger)
        {
            logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lazyLogger = new Lazy<ILogger>(() => logger);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        /// <param name="logTarget">Optional. The log target type. Defaults to this type.</param>
        protected Loggable(ILogManager? logManager, Type? logTarget = null)
        {
            this.lazyLogger = new Lazy<ILogger>(
                () => logManager?.GetLogger(logTarget ?? this.GetType())
                        ?? (logTarget ?? this.GetType()).GetLogger());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="logManagerGetter">The log manager getter.</param>
        /// <param name="logTarget">Optional. The log target type. Defaults to this type.</param>
        protected Loggable(Func<ILogManager?> logManagerGetter, Type? logTarget = null)
        {
            this.lazyLogger = new Lazy<ILogger>(
                () => logManagerGetter?.Invoke()?.GetLogger(logTarget ?? this.GetType())
                        ?? (logTarget ?? this.GetType()).GetLogger());
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger Logger
        {
            get => this.lazyLogger.Value;
            protected internal set
            {
                value = value ?? throw new ArgumentNullException(nameof(value));
                this.lazyLogger = new Lazy<ILogger>(() => value);
            }
        }
    }
}