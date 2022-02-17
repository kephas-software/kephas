// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRef.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceRef interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;

    /// <summary>
    /// Contract used to define the type of properties referencing services.
    /// </summary>
    /// <remarks>
    /// This type of reference is used typically when the referenced service allows multiple implementations
    /// and the entity containing the property references one of these implementations.
    /// </remarks>
    public interface IServiceRef
    {
        /// <summary>
        /// Gets or sets the name of the referenced service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        string? ServiceName { get; set; }

        /// <summary>
        /// Gets the type of the referenced service.
        /// </summary>
        /// <value>
        /// The type of the referenced service.
        /// </value>
        Type ServiceType { get; }

        /// <summary>
        /// Gets the referenced service.
        /// </summary>
        /// <returns>
        /// The referenced service or <c>null</c>.
        /// </returns>
        object? GetService();
    }

    /// <summary>
    /// Generic contract used to define the type of properties referencing services.
    /// </summary>
    /// <typeparam name="TService">Type of the referenced service.</typeparam>
    public interface IServiceRef<out TService> : IServiceRef
        where TService : class
    {
        /// <summary>
        /// Gets the referenced service.
        /// </summary>
        /// <returns>
        /// The referenced service or <c>null</c>.
        /// </returns>
        new TService? GetService();
    }
}