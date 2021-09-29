// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISystemCompositionPartBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Conventions
{
    using System.Composition.Hosting;

    using Kephas.Injection.Conventions;

    /// <summary>
    /// Part builder interface specific to System.Composition.
    /// </summary>
    public interface ISystemCompositionPartBuilder : IPartBuilder
    {
        /// <summary>
        /// Sets the container up using the configuration.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        void Build(ContainerConfiguration configuration);
    }
}