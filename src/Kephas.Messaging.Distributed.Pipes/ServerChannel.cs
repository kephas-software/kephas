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
        private readonly Func<ServerChannel, string, CancellationToken, Task> messageReceived;
        private readonly CancellationToken cancellationToken;
        private Task? listenTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerChannel"/> class.
        /// </summary>
        /// <param name="channelName">The channel name.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="messageReceived">The callback for when the message is received.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ServerChannel(
            string channelName,
            ILogger logger,
            Func<ServerChannel, string, CancellationToken, Task> messageReceived,
            CancellationToken cancellationToken)
            : base(channelName, logger)
        {
            this.messageReceived = messageReceived;
            this.cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Opens the server channel.
        /// </summary>
        public virtual void Open()
        {
            this.listenTask = this.StartListeningAsync(this.messageReceived);
        }

        private async Task StartListeningAsync(Func<ServerChannel, string, CancellationToken, Task> messageReceived)
        {
            await Task.Yield();

            var channelName = this.ChannelName;

            using var stream = new NamedPipeServerStream(
                this.ChannelName,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous);

            this.Logger.Debug("Waiting for connection on {channel} (server mode)...", channelName);

            var connectSuccessful = false;
            try
            {
                await stream.WaitForConnectionAsync(this.cancellationToken).PreserveThreadContext();

                this.Logger.Debug("Client connected on {channel} (server mode).", channelName);

                this.cancellationToken.ThrowIfCancellationRequested();

                connectSuccessful = true;
            }
            catch (OperationCanceledException)
                when (this.cancellationToken.IsCancellationRequested)
            {
                this.Logger.Warn("Connection cancelled while processing the request from client on {channel}.", channelName);
                return;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while waiting for connection on {channel}.", channelName);
            }

            // client connection is successful, prepare for the next client connection.
            this.listenTask = this.StartListeningAsync(messageReceived);

            if (!connectSuccessful)
            {
                // if the client connection is not successful, skip reading from the channel.
                return;
            }

            try
            {
                this.cancellationToken.ThrowIfCancellationRequested();

                var messageString = await this.ReadAsync(stream).PreserveThreadContext();

                if (this.Logger.IsDebugEnabled())
                {
                    this.Logger.Debug(
                        "Message arrived on {channel} (server mode): '{message}'.",
                        channelName,
                        messageString[..50] + "...");
                }

                await messageReceived(this, messageString, this.cancellationToken).PreserveThreadContext();
            }
            catch (OperationCanceledException)
                when (this.cancellationToken.IsCancellationRequested)
            {
                this.Logger.Warn("Read cancelled while processing the request from client on {channel}.", channelName);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while processing the request from client on {channel}.", channelName);
            }
        }

        private async Task<string> ReadAsync(PipeStream stream)
        {
            var buffer = new byte[2048];
            using var memStream = new MemoryStream(4096);
            do
            {
                this.cancellationToken.ThrowIfCancellationRequested();

                var numBytes = await stream.ReadAsync(buffer, 0, buffer.Length, this.cancellationToken).PreserveThreadContext();
                memStream.Write(buffer, 0, numBytes);
            }
            while (!this.cancellationToken.IsCancellationRequested && !stream.IsMessageComplete);

            memStream.Position = 0;
            var bytes = await memStream.ReadAllBytesAsync(this.cancellationToken).PreserveThreadContext();

            return Encoding.UTF8.GetString(bytes);
        }
    }
}