// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppIdentity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the app identity class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    using Kephas.Data;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// An app identity.
    /// </summary>
    public class AppIdentity : Expando, IIdentifiable, IEquatable<AppIdentity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppIdentity"/> class.
        /// </summary>
        /// <param name="id">The app ID.</param>
        /// <param name="version">Optional. The version.</param>
        public AppIdentity(string id, string version = null)
        {
            Requires.NotNullOrEmpty(id, nameof(id));

            this.Id = id;
            this.Version = version;
        }

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        object IIdentifiable.Id => this.ToString();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(AppIdentity other)
        {
            return other != null && this.ToString() == other.ToString();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj) || this.Equals(obj as AppIdentity);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.ToLower().GetHashCode() + (100 * (this.Version?.ToLower().GetHashCode() ?? 0));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Version) ? this.Id : $"{this.Id}:{this.Version}";
        }
    }
}
