// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISystemCompositionRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Builder
{
    using System.Composition.Hosting;
    using Kephas.Injection.Builder;

    /// <summary>
    /// Part builder interface specific to System.Composition.
    /// </summary>
    public interface ISystemCompositionRegistrationBuilder : IRegistrationBuilder
    {
        /// <summary>
        /// Sets the container up using the configuration.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        void Build(ContainerConfiguration configuration);
    }
}