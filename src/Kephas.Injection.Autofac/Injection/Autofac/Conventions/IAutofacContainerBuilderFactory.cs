// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAutofacContainerBuilderFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAutofacContainerBuilderFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Conventions
{
    using global::Autofac;

    /// <summary>
    /// Interface for Autofac container builder provider.
    /// </summary>
    public interface IAutofacContainerBuilderFactory
    {
        /// <summary>
        /// Gets the container builder.
        /// </summary>
        /// <returns>
        /// The container builder.
        /// </returns>
        ContainerBuilder CreateContainerBuilder();
    }
}