// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes
{
    using Kephas.Logging;

    /// <summary>
    /// Base class for pipe.
    /// </summary>
    internal abstract class ChannelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelBase"/> class.
        /// </summary>
        /// <param name="channelName">The channel name.</param>
        /// <param name="logger">The logger.</param>
        protected ChannelBase(string channelName, ILogger logger)
        {
            this.ChannelName = channelName;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the channel name.
        /// </summary>
        public string ChannelName { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }
    }
}