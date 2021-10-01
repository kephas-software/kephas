// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRegistrationBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Interface for part builder.
    /// </summary>
    public interface IRegistrationBuilder
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
        IRegistrationBuilder As(Type contractType);

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
        IRegistrationBuilder As<TContract>() => this.As(typeof(TContract));

        /// <summary>
        /// Registers the service as a singleton.
        /// </summary>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IRegistrationBuilder Singleton();

        /// <summary>
        /// Registers the service as scoped.
        /// </summary>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IRegistrationBuilder Scoped();

        /// <summary>
        /// Registers the service with multiple instances.
        /// </summary>
        /// <param name="value">Optional. True if multiple service registrations are allowed (default), false otherwise.</param>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IRegistrationBuilder AllowMultiple(bool value = true);

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param>
        /// <param name="parameterBuilder">The parameter builder.</param>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IRegistrationBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IParameterBuilder>? parameterBuilder = null);

        /// <summary>
        /// Adds metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IRegistrationBuilder AddMetadata(string name, object? value);

        /// <summary>
        /// Adds metadata to the export.
        /// </summary>
        /// <param name="metadata">The metadata dictionary.</param>
        /// <returns>
        /// A registration builder allowing further configuration.
        /// </returns>
        IRegistrationBuilder AddMetadata(IDictionary<string, object?> metadata)
        {
            foreach (var (key, value) in metadata)
            {
                this.AddMetadata(key, value);
            }

            return this;
        }
    }
}