// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerChannel.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes
{
    using System;
    using System.IO;
    using System.IO.Pipes;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.IO;
    using Kephas.Logging;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Server side pipe.
    /// </summary>
    internal class ServerChannel : ChannelBase
    {
        private readonly Func<ServerChannel, CancellationToken, Task> connectionEstablished;
        private readonly Func<ServerChannel, string, CancellationToken, Task> messageReceived;
        private readonly CancellationToken cancellationToken;
        private Task? listenTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerChannel"/> class.
        /// </summary>
        /// <param name="appInstanceId">The application instance ID.</param>
        /// <param name="channelName">The channel name.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="connectionEstablished">The callback for when the connection with the client is established.</param>
        /// <param name="messageReceived">The callback for when the message is received.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ServerChannel(
            string appInstanceId,
            string channelName,
            ILogger logger,
            Func<ServerChannel, CancellationToken, Task> connectionEstablished,
            Func<ServerChannel, string, CancellationToken, Task> messageReceived,
            CancellationToken cancellationToken)
            : base(
                appInstanceId,
                channelName,
                logger)
        {
            this.connectionEstablished = connectionEstablished;
            this.messageReceived = messageReceived;
            this.cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Opens the server channel.
        /// </summary>
        public virtual void Open()
        {
            this.Stream = new NamedPipeServerStream(
                this.ChannelName,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous);
            this.listenTask = this.StartListeningAsync(this.connectionEstablished, this.messageReceived, this.cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Stream?.WaitForPipeDrain();
            }

            base.Dispose(disposing);
        }

        private async Task StartListeningAsync(
            Func<ServerChannel, CancellationToken, Task> connectionEstablished,
            Func<ServerChannel, string, CancellationToken, Task> messageReceived,
            CancellationToken cancellationToken)
        {
            await Task.Yield();

            var channelName = this.ChannelName;
            this.Logger.Debug("Waiting for connection on {channel} (server mode)...", channelName);

            await ((NamedPipeServerStream)this.Stream).WaitForConnectionAsync(cancellationToken).PreserveThreadContext();

            this.Logger.Debug("Client connected on {channel} (server mode).", channelName);

            await connectionEstablished(this, cancellationToken).PreserveThreadContext();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var messageString = await this.ReadAsync(cancellationToken).PreserveThreadContext();

                    if (this.Logger.IsDebugEnabled())
                    {
                        this.Logger.Debug("Message arrived on {channel} (server mode): '{message}'.", channelName, messageString.Substring(0, 50) + "...");
                    }

                    await messageReceived(this, messageString, cancellationToken).PreserveThreadContext();
                }
                catch (OperationCanceledException)
                    when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Error while processing the request from client.");
                }
            }
        }

        /// <summary>
        /// Reads a message from the pipe asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result yielding the read message.</returns>
        private async Task<string> ReadAsync(CancellationToken cancellationToken)
        {
            var channel = this.Stream!;
            var buffer = new byte[256];
            using var memStream = new MemoryStream(1000);
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var numBytes = await channel.ReadAsync(buffer, 0, buffer.Length, cancellationToken).PreserveThreadContext();
                memStream.Write(buffer, 0, numBytes);
            }
            while (!cancellationToken.IsCancellationRequested && !channel.IsMessageComplete);

            memStream.Position = 0;
            var bytes = memStream.ReadAllBytes();

            return Encoding.UTF8.GetString(bytes);
        }
    }
}