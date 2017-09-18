// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpClientMessageProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the HTTP client message processor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Client.Http
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A HTTP client message processor.
    /// </summary>
    public class HttpClientMessageProcessor : IClientMessageProcessor
    {
        /// <summary>
        /// Processes the specified message asynchronously by sending it to the server and waiting for a response.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public Task<IMessage> ProcessAsync(IMessage message, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Processes the specified message asynchronously by sending it to the server without waiting for a response.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessOneWay(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates the HTTP client.
        /// </summary>
        /// <returns>An instance of <see cref="HttpClient"/>.</returns>
        protected virtual HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            // TODO ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true; // for testing only, enforces trust to server certificate.
            return client;
        }
    }
}