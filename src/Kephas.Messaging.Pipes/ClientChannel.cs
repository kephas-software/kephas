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
    using Kephas.Threading;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Client side pipe.
    /// </summary>
    internal class ClientChannel : ChannelBase
    {
        private readonly IConfiguration<PipesSettings> pipesConfiguration;
        private readonly Lock writeLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientChannel"/> class.
        /// </summary>
        /// <param name="appInstanceId">The application instance ID.</param>
        /// <param name="serverName">The server name.</param>
        /// <param name="channelName">The channel name.</param>
        /// <param name="pipesConfiguration">The pipes configuration.</param>
        /// <param name="logger">The logger.</param>
        public ClientChannel(string appInstanceId, string? serverName, string channelName, IConfiguration<PipesSettings> pipesConfiguration, ILogger logger)
            : base(appInstanceId, channelName, logger)
        {
            this.pipesConfiguration = pipesConfiguration;
            this.ServerName = serverName ?? ".";
            this.writeLock = new Lock();
        }

        /// <summary>
        /// Gets the server name.
        /// </summary>
        public string ServerName { get; }

        /// <summary>
        /// Gets a value indicating whether the channel is to itself.
        /// </summary>
        public virtual bool IsSelf => false;

        /// <summary>
        /// Opens the channel asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual async Task OpenAsync(CancellationToken cancellationToken)
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
                this.Stream = stream;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while connecting to {server}/{channel} (client mode).", this.ServerName, this.ChannelName);
                throw;
            }
        }

        /// <summary>
        /// Writes the bytes to the pipe asynchronously.
        /// </summary>
        /// <param name="message">The message being sent.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public async Task WriteAsync(string message, CancellationToken cancellationToken)
        {
            var bytes = Encoding.UTF8.GetBytes(message);

            await this.writeLock.EnterAsync(async () =>
            {
                try
                {
                    this.Logger.Trace("Writing message ({messageLength}) to {channel}...", bytes.Length, this.ChannelName);

                    await this.Stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).PreserveThreadContext();
                    await this.Stream.FlushAsync(cancellationToken).PreserveThreadContext();

                    this.Logger.Trace("Written message ({messageLength}) to {channel}.", bytes.Length, this.ChannelName);
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Error while writing message ({messageLength}) to {channel}.", bytes.Length, this.ChannelName);
                    throw;
                }
            }).PreserveThreadContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.writeLock.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    internal class SelfClientChannel : ClientChannel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfClientChannel"/> class.
        /// </summary>
        /// <param name="appInstanceId">The application instance ID.</param>
        /// <param name="serverName">The server name.</param>
        /// <param name="channelName">The channel name.</param>
        /// <param name="pipesConfiguration">The pipes configuration.</param>
        /// <param name="logger">The logger.</param>
        public SelfClientChannel(string appInstanceId, string? serverName, string channelName, IConfiguration<PipesSettings> pipesConfiguration, ILogger logger)
            : base(appInstanceId, serverName, channelName, pipesConfiguration, logger)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the channel is to itself.
        /// </summary>
        public override bool IsSelf => true;

        /// <summary>
        /// Opens the channel asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}