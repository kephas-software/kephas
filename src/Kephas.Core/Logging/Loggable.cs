﻿// --------------------------------------------------------------------------------------------------------------------
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
    public class Loggable : ILoggable
    {
        private static ILogManager logManager = new NullLogManager();

        private Lazy<ILogger> lazyLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        public Loggable()
            : this(() => DefaultLogManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public Loggable(IAmbientServices ambientServices)
            : this(() => ambientServices?.LogManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        public Loggable(ICompositionContext compositionContext)
            : this(() => compositionContext?.GetExport<ILogManager>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        public Loggable(IContextFactory contextFactory)
            : this(() => contextFactory?.GetLogManager())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public Loggable(ILogManager logManager)
        {
            this.lazyLogger = new Lazy<ILogger>(
                () => logManager?.GetLogger(this.GetType())
                        ?? LoggingExtensions.GetLogger(this, null));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loggable"/> class.
        /// </summary>
        /// <param name="logManagerGetter">The log manager getter.</param>
        public Loggable(Func<ILogManager> logManagerGetter)
        {
            this.lazyLogger = new Lazy<ILogger>(
                () => logManagerGetter?.Invoke()?.GetLogger(this.GetType())
                        ?? LoggingExtensions.GetLogger(this, null));
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
            get => this.lazyLogger.Value;
            protected internal set
            {
                Requires.NotNull(value, nameof(value));
                this.lazyLogger = new Lazy<ILogger>(() => value);
            }
        }
    }
}