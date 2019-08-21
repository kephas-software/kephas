// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAutofacContainerBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAutofacContainerBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Conventions
{
    using System;
    using System.Collections.Generic;

    using global::Autofac;

    /// <summary>
    /// Interface for Autofac container builder.
    /// </summary>
    public interface IAutofacContainerBuilder
    {
        /// <summary>
        /// Configures the container builder with the given parts.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// The container builder.
        /// </returns>
        ContainerBuilder GetContainerBuilder(IEnumerable<Type> parts);
    }
}