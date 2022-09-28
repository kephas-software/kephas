// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceInfoBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceInfoBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    using System;
    using System.Collections.Generic;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Interface for part builder.
    /// </summary>
    public interface IAppServiceInfoBuilder
    {
        /// <summary>
        /// Sets the registration contract.
        /// </summary>
        /// <remarks>
        /// The registration contract is the key to find the service.
        /// The registered service type is a subtype providing additional information, typically metadata.
        /// </remarks>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IAppServiceInfoBuilder As(Type contractType);

        /// <summary>
        /// Sets the registration contract.
        /// </summary>
        /// <remarks>
        /// The registration contract is the key to find the service.
        /// The registered service type is a subtype providing additional information, typically metadata.
        /// </remarks>
        /// <typeparam name="TContract">The contract type.</typeparam>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IAppServiceInfoBuilder As<TContract>() => this.As(typeof(TContract));

        /// <summary>
        /// Registers the service as a singleton.
        /// </summary>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IAppServiceInfoBuilder Singleton();

        /// <summary>
        /// Registers the service as scoped.
        /// </summary>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IAppServiceInfoBuilder Scoped();

        /// <summary>
        /// Registers the service as transient.
        /// </summary>
        /// <remarks>
        /// By default, typical dependency injection frameworks use
        /// transient registration, so this default implementation does nothing.
        /// Override it for specific needs.
        /// </remarks>
        /// <returns>
        /// This builder.
        /// </returns>
        IAppServiceInfoBuilder Transient() => this;

        /// <summary>
        /// Registers the service with multiple instances.
        /// </summary>
        /// <param name="value">Optional. True if multiple service registrations are allowed (default), false otherwise.</param>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IAppServiceInfoBuilder AllowMultiple(bool value = true);

        /// <summary>
        /// Adds metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IAppServiceInfoBuilder AddMetadata(string name, object? value);

        /// <summary>
        /// Adds metadata to the export.
        /// </summary>
        /// <param name="metadata">The metadata dictionary.</param>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IAppServiceInfoBuilder AddMetadata(IDictionary<string, object?> metadata)
        {
            foreach (var (key, value) in metadata)
            {
                this.AddMetadata(key, value);
            }

            return this;
        }

        /// <summary>
        /// Indicates whether the created instances are disposed by an external owner.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        IAppServiceInfoBuilder ExternallyOwned();

        /// <summary>
        /// Builds the <see cref="IAppServiceInfo"/> instance.
        /// </summary>
        /// <returns>The <see cref="IAppServiceInfo"/> instance.</returns>
        IAppServiceInfo Build();
    }
}