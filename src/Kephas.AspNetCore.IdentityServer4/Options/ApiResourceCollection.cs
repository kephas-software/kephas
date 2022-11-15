// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiResourceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Options
{
    using System;
    using System.Collections.Generic;

    using global::IdentityServer4.Models;

    /// <summary>
    /// A collection of <see cref="ApiResource"/>.
    /// </summary>
    public class ApiResourceCollection : ResourceCollectionBase<ApiResource>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResourceCollection"/> class.
        /// </summary>
        public ApiResourceCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResourceCollection"/> class with the given
        /// API resources in <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The initial list of <see cref="ApiResource"/>.</param>
        public ApiResourceCollection(IList<ApiResource> list)
            : base(list)
        {
        }

        /// <summary>
        /// Adds a new externally registered API.
        /// </summary>
        /// <param name="name">The name of the API.</param>
        /// <param name="configure">The <see cref="Action{ApiResourceBuilder}"/> to configure the externally registered API.</param>
        public void AddApiResource(string name, Action<ApiResourceBuilder> configure)
        {
            var apiResource = ApiResourceBuilder.ApiResource(name);
            configure(apiResource);
            Add(apiResource.Build());
        }

        /// <summary>
        /// Creates a new API that coexists with an authorization server.
        /// </summary>
        /// <param name="name">The name of the API.</param>
        /// <param name="configure">The <see cref="Func{ApiResourceBuilder, ApiResource}"/> to configure the identity server jwt API.</param>
        public void AddIdentityServerJwt(string name, Action<ApiResourceBuilder> configure)
        {
            var apiResource = ApiResourceBuilder.IdentityServerJwt(name);
            configure(apiResource);
            Add(apiResource.Build());
        }
    }
}
