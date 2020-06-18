// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientChannel.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes
{
    using System;
    using System.IO.Pipes;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Messaging.Pipes.Configuration;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Client side pipe.
    /// </summary>
    internal class ClientChannel : ChannelBase
    {
        private readonly IConfiguration<PipesSettings> pipesConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientChannel"/> class.
        /// </summary>
        /// <param name="appInstanceId">The application instance ID.</param>
        /// <param name="serverName">The server name.</param>
        /// <param name="channelName">The channel name.</param>
        /// <param name="pipesConfiguration">The pipes configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="isSelf">Indicates whether this is a dummy channel to itself.</param>
        public ClientChannel(
            string appInstanceId,
            string? serverName,
            string channelName,
            IConfiguration<PipesSettings> pipesConfiguration,
            ILogger logger,
            bool isSelf)
            : base(channelName, logger)
        {
            this.pipesConfiguration = pipesConfiguration;
            this.AppInstanceId = appInstanceId;
            this.ServerName = serverName ?? ".";
            this.IsSelf = isSelf;
        }

        /// <summary>
        /// Gets the application instance ID.
        /// </summary>
        public string AppInstanceId { get; }

        /// <summary>
        /// Gets the server name.
        /// </summary>
        public string ServerName { get; }

        /// <summary>
        /// Gets a value indicating whether the channel is to itself.
        /// </summary>
        public virtual bool IsSelf { get; }

        /// <summary>
        /// Writes the bytes to the pipe asynchronously.
        /// </summary>
        /// <param name="message">The message being sent.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual async Task WriteAsync(string message, CancellationToken cancellationToken)
        {
            if (this.IsSelf)
            {
                throw new PipesMessagingException($"Cannot write messages in a {this.GetType()} channel.");
            }

            var bytes = Encoding.UTF8.GetBytes(message);

            using var stream = await this.OpenAsync(cancellationToken).PreserveThreadContext();

            try
            {
                this.Logger.Trace("Writing message ({messageLength}) to {channel}...", bytes.Length, this.ChannelName);

                if (!stream.IsConnected)
                {
                    throw new PipesMessagingException($"{this.ChannelName} disconnected from server.");
                }

                await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).PreserveThreadContext();
                await stream.FlushAsync(cancellationToken).PreserveThreadContext();
                stream.WaitForPipeDrain();

                this.Logger.Trace("Written message ({messageLength}) to {channel}.", bytes.Length, this.ChannelName);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while writing message ({messageLength}) to {channel}.", bytes.Length, this.ChannelName);
                throw;
            }
        }

        /// <summary>
        /// Opens the channel asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result yielding the open pipe stream.</returns>
        private async Task<NamedPipeClientStream> OpenAsync(CancellationToken cancellationToken)
        {
            var stream = new NamedPipeClientStream(
                this.ServerName,
                this.ChannelName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous);

            try
            {
                this.Logger.Debug("Connecting to pipe {server}/{channel} (client mode)...", this.ServerName, this.ChannelName);

                await stream.ConnectAsync((int)this.pipesConfiguration.Settings.ConnectionTimeout.TotalMilliseconds, cancellationToken).PreserveThreadContext();
                stream.ReadMode = PipeTransmissionMode.Message;

                this.Logger.Debug("Connected to pipe {server}/{channel} (client mode).", this.ServerName, this.ChannelName);
                return stream;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while connecting to {server}/{channel} (client mode).", this.ServerName, this.ChannelName);
                throw;
            }
        }
    }
}