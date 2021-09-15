// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAutofacContainerBuilderProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAutofacContainerBuilderProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Conventions
{
    using global::Autofac;

    /// <summary>
    /// Interface for Autofac container builder provider.
    /// </summary>
    public interface IAutofacContainerBuilderProvider
    {
        /// <summary>
        /// Gets the container builder.
        /// </summary>
        /// <returns>
        /// The container builder.
        /// </returns>
        ContainerBuilder GetContainerBuilder();
    }
}