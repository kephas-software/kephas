// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes
{
    using System;
    using System.IO.Pipes;

    using Kephas.Logging;

    /// <summary>
    /// Base class for pipe.
    /// </summary>
    internal abstract class ChannelBase : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelBase"/> class.
        /// </summary>
        /// <param name="appInstanceId">The application instance ID.</param>
        /// <param name="channelName">The channel name.</param>
        /// <param name="logger">The logger.</param>
        protected ChannelBase(string appInstanceId, string channelName, ILogger logger)
        {
            this.AppInstanceId = appInstanceId;
            this.ChannelName = channelName;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the application instance ID.
        /// </summary>
        public string AppInstanceId { get; }

        /// <summary>
        /// Gets the channel name.
        /// </summary>
        public string ChannelName { get; }

        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        public PipeStream? Stream { get; protected set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Stream?.Dispose();
            }
        }
    }
}