// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPartBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPartBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Interface for part builder.
    /// </summary>
    public interface IPartBuilder
    {
        /// <summary>
        /// Indicates the type registered as the exported service key.
        /// </summary>
        /// <param name="contractType">Type of the service.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartBuilder As(Type contractType);

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        IPartBuilder Singleton();

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartBuilder Scoped();

        /// <summary>
        /// Indicates that this service allows multiple registrations.
        /// </summary>
        /// <param name="value">True if multiple service registrations are allowed, false otherwise.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartBuilder AllowMultiple(bool value);

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param><param name="importConfiguration">Action configuring the parameters of the selected constructor.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder>? importConfiguration = null);

        /// <summary>
        /// Adds metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// A part builder allowing further configuration.
        /// </returns>
        IPartBuilder AddMetadata(string name, object? value);

        /// <summary>
        /// Adds metadata to the export.
        /// </summary>
        /// <param name="metadata">The metadata dictionary.</param>
        /// <returns>
        /// A part builder allowing further configuration.
        /// </returns>
        IPartBuilder AddMetadata(IDictionary<string, object?> metadata)
        {
            foreach (var (key, value) in metadata)
            {
                this.AddMetadata(key, value);
            }

            return this;
        }
    }
}