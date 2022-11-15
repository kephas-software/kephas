// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRouterAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message router attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    using System;
    using System.Collections.Generic;

    using Kephas.Injection;

    /// <summary>
    /// Attribute for message router.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MessageRouterAttribute : Attribute, IMetadataProvider
    {
        /// <summary>
        /// Gets or sets the receiver URL regular expression.
        /// </summary>
        /// <value>
        /// The receiver URL regular expression.
        /// </value>
        public string? ReceiverMatch { get; set; }

        /// <summary>
        /// Gets or sets the type of the receiver match provider.
        /// </summary>
        /// <value>
        /// The type of the receiver match provider.
        /// </value>
        public Type? ReceiverMatchProviderType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this router is the fallback router.
        /// </summary>
        /// <value>
        /// True if this router is fallback, false if not.
        /// </value>
        public bool IsFallback { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the router is optional.
        /// Optional routers which cannot be initialized are simply ignored.
        /// </summary>
        /// <value>
        /// True if the router is optional, false otherwise.
        /// </value>
        public bool IsOptional { get; set; } = false;

        /// <summary>
        /// Gets the metadata as an enumeration of (name, value) pairs.
        /// </summary>
        /// <returns>An enumeration of (name, value) pairs.</returns>
        public IEnumerable<(string name, object? value)> GetMetadata()
        {
            yield return (nameof(MessageRouterMetadata.ReceiverMatch), this.ReceiverMatch);
            yield return (nameof(MessageRouterMetadata.ReceiverMatchProviderType), this.ReceiverMatchProviderType);
            yield return (nameof(MessageRouterMetadata.IsFallback), this.IsFallback);
            yield return (nameof(MessageRouterMetadata.IsOptional), this.IsOptional);
        }
    }
}
