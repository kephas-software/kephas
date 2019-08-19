// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediServiceProviderBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMediServiceProviderBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Medi.Conventions
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Interface for Microsoft.Extensions.DependencyInjection service provider builder.
    /// </summary>
    public interface IMediServiceProviderBuilder
    {
        /// <summary>
        /// Builds service provider.
        /// </summary>
        /// <param name="parts">The parts being built.</param>
        /// <returns>
        /// A ServiceProvider.
        /// </returns>
        ServiceProvider BuildServiceProvider(IEnumerable<Type> parts);
    }
}