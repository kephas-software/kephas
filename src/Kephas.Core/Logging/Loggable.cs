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

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
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
        /// <param name="ambientServices">The ambient services.</param>
        protected Loggable(IAmbientServices ambientServices)
            : this(() => ambientServices?.LogManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        protected Loggable(ICompositionContext compositionContext)
            : this(() => compositionContext?.GetExport<ILogManager>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        protected Loggable(IContextFactory contextFactory)
            : this(() => contextFactory?.GetLogManager())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        protected Loggable(ILogManager? logManager)
        {
            this.lazyLogger = new Lazy<ILogger>(
                () => logManager?.GetLogger(this.GetType())
                        ?? LoggingExtensions.GetLogger(this, null));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="logManagerGetter">The log manager getter.</param>
        protected Loggable(Func<ILogManager> logManagerGetter)
        {
            this.lazyLogger = new Lazy<ILogger>(
                () => logManagerGetter?.Invoke()?.GetLogger(this.GetType())
                        ?? LoggingExtensions.GetLogger(this, null));
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
                Requires.NotNull(value, nameof(value));
                this.lazyLogger = new Lazy<ILogger>(() => value);
            }
        }
    }
}