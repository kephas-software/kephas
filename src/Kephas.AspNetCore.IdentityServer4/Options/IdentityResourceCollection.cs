// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityResourceCollection.cs" company="Kephas Software SRL">
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
    /// A collection of <see cref="IdentityResource"/>.
    /// </summary>
    public class IdentityResourceCollection : ResourceCollectionBase<IdentityResource>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityResourceCollection"/> class.
        /// </summary>
        public IdentityResourceCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityResourceCollection"/> class with the given
        /// identity resources in <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The initial list of <see cref="IdentityResource"/>.</param>
        public IdentityResourceCollection(IList<IdentityResource> list)
            : base(list)
        {
        }

        /// <summary>
        /// Adds an openid resource.
        /// </summary>
        public void AddOpenId() =>
            this.Add(IdentityResourceBuilder.OpenId().Build());

        /// <summary>
        /// Adds an openid resource.
        /// </summary>
        /// <param name="configure">The <see cref="Action{IdentityResourceBuilder}"/> to configure the openid scope.</param>
        public void AddOpenId(Action<IdentityResourceBuilder> configure)
        {
            var resource = IdentityResourceBuilder.OpenId();
            configure(resource);
            this.Add(resource.Build());
        }

        /// <summary>
        /// Adds a profile resource.
        /// </summary>
        public void AddProfile() =>
            this.Add(IdentityResourceBuilder.Profile().Build());

        /// <summary>
        /// Adds a profile resource.
        /// </summary>
        /// <param name="configure">The <see cref="Action{IdentityResourceBuilder}"/> to configure the profile scope.</param>
        public void AddProfile(Action<IdentityResourceBuilder> configure)
        {
            var resource = IdentityResourceBuilder.Profile();
            configure(resource);
            this.Add(resource.Build());
        }

        /// <summary>
        /// Adds an address resource.
        /// </summary>
        public void AddAddress() =>
            this.Add(IdentityResourceBuilder.Address().Build());

        /// <summary>
        /// Adds an address resource.
        /// </summary>
        /// <param name="configure">The <see cref="Action{IdentityResourceBuilder}"/> to configure the address scope.</param>
        public void AddAddress(Action<IdentityResourceBuilder> configure)
        {
            var resource = IdentityResourceBuilder.Address();
            configure(resource);
            this.Add(resource.Build());
        }

        /// <summary>
        /// Adds an email resource.
        /// </summary>
        public void AddEmail() =>
            this.Add(IdentityResourceBuilder.Email().Build());

        /// <summary>
        /// Adds an email resource.
        /// </summary>
        /// <param name="configure">The <see cref="Action{IdentityResourceBuilder}"/> to configure the email scope.</param>
        public void AddEmail(Action<IdentityResourceBuilder> configure)
        {
            var resource = IdentityResourceBuilder.Email();
            configure(resource);
            this.Add(resource.Build());
        }

        /// <summary>
        /// Adds a phone resource.
        /// </summary>
        public void AddPhone() =>
            this.Add(IdentityResourceBuilder.Phone().Build());

        /// <summary>
        /// Adds a phone resource.
        /// </summary>
        /// <param name="configure">The <see cref="Action{IdentityResourceBuilder}"/> to configure the phone scope.</param>
        public void AddPhone(Action<IdentityResourceBuilder> configure)
        {
            var resource = IdentityResourceBuilder.Phone();
            configure(resource);
            this.Add(resource.Build());
        }
    }
}
