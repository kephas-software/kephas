// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IInjectorBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    /// <summary>
    /// Contract for injection builders.
    /// </summary>
    public interface IInjectorBuilder
    {
        /// <summary>
        /// Creates the injector.
        /// </summary>
        /// <returns>The newly created injector.</returns>
        IInjector CreateInjector();
    }
}