// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.SignalR
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Microsoft.AspNetCore.SignalR;

    /// <summary>
    /// Abstract base class for hubs registered for auto registration.
    /// </summary>
    public abstract class HubService : Hub, IHubService
    {
        private Lazy<ILogger> lazyLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HubService"/> class.
        /// </summary>
        /// <param name="logManager">Optional. The log manager.</param>
        protected HubService(ILogManager? logManager = null)
        {
            this.lazyLogger = new Lazy<ILogger>(
                () => logManager?.GetLogger(this.GetType())
                      ?? this.GetType().GetLogger());
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

    /// <summary>
    /// Abstract base class for hubs registered for auto registration.
    /// </summary>
    /// <typeparam name="T">The type of client.</typeparam>
    public abstract class HubService<T> : Hub<T>, IHubService
        where T : class
    {
        private Lazy<ILogger> lazyLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HubService{T}"/> class.
        /// </summary>
        /// <param name="logManager">Optional. The log manager.</param>
        protected HubService(ILogManager? logManager = null)
        {
            this.lazyLogger = new Lazy<ILogger>(
                () => logManager?.GetLogger(this.GetType())
                      ?? this.GetType().GetLogger());
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