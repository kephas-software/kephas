// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHasInjectionBuildContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    /// <summary>
    /// Provides the <see cref="BuildContext"/> property.
    /// </summary>
    public interface IHasInjectionBuildContext
    {
        /// <summary>
        /// Gets the <see cref="IInjectionBuildContext"/>.
        /// </summary>
        IInjectionBuildContext BuildContext { get; }
    }
}