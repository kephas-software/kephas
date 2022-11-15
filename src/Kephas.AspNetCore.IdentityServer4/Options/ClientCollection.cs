// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Options
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using global::IdentityServer4.Models;

    /// <summary>
    /// A collection of <see cref="Client"/>.
    /// </summary>
    public class ClientCollection : Collection<Client>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCollection"/> class.
        /// </summary>
        public ClientCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCollection"/> class with the given
        /// clients in <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The initial list of <see cref="Client"/>.</param>
        public ClientCollection(IList<Client> list)
            : base(list)
        {
        }

        /// <summary>
        /// Gets a client given its client id.
        /// </summary>
        /// <param name="key">The name of the <see cref="Client"/>.</param>
        /// <returns>The <see cref="Client"/>.</returns>
        public Client this[string key]
        {
            get
            {
                foreach (var candidate in this.Items)
                {
                    if (string.Equals(candidate.ClientId, key, StringComparison.Ordinal))
                    {
                        return candidate;
                    }
                }

                throw new KeyNotFoundException($"Client '{key}' not found.");
            }
        }

        /// <summary>
        /// Adds the clients in <paramref name="clients"/> to the collection.
        /// </summary>
        /// <param name="clients">The list of <see cref="Client"/> to add.</param>
        public void AddRange(params Client[] clients)
        {
            foreach (var client in clients)
            {
                this.Add(client);
            }
        }

        /// <summary>
        /// Adds a single page application that coexists with an authorization server.
        /// </summary>
        /// <param name="clientId">The client id for the single page application.</param>
        /// <param name="configure">The <see cref="Action{ClientBuilder}"/> to configure the default single page application.</param>
        /// <returns>The newly added client.</returns>
        public Client AddIdentityServerSPA(string clientId, Action<ClientBuilder>? configure = null)
        {
            var app = ClientBuilder.IdentityServerSPA(clientId);
            configure?.Invoke(app);
            var client = app.Build();
            this.Add(client);
            return client;
        }

        /// <summary>
        /// Adds an externally registered single page application.
        /// </summary>
        /// <param name="clientId">The client id for the single page application.</param>
        /// <param name="configure">The <see cref="Action{ClientBuilder}"/> to configure the default single page application.</param>
        /// <returns>The newly added client.</returns>
        public Client AddSPA(string clientId, Action<ClientBuilder>? configure = null)
        {
            var app = ClientBuilder.SPA(clientId);
            configure?.Invoke(app);
            var client = app.Build();
            this.Add(client);
            return client;
        }

        /// <summary>
        /// Adds an externally registered native application..
        /// </summary>
        /// <param name="clientId">The client id for the single page application.</param>
        /// <param name="configure">The <see cref="Action{ClientBuilder}"/> to configure the native application.</param>
        /// <returns>The newly added client.</returns>
        public Client AddNativeApp(string clientId, Action<ClientBuilder>? configure = null)
        {
            var app = ClientBuilder.NativeApp(clientId);
            configure?.Invoke(app);
            var client = app.Build();
            this.Add(client);
            return client;
        }
    }
}
