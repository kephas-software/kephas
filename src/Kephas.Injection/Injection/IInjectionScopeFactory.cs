// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionScopeFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using Kephas.Services;

    /// <summary>
    /// Public interface for factories of <see cref="IInjectionScope"/>.
    /// </summary>
    /// <remarks>
    /// The registration is transient
    /// </remarks>
    [AppServiceContract]
    public interface IInjectionScopeFactory
    {
        /// <summary>
        /// Creates a disposable injection scope.
        /// </summary>
        /// <returns>
        /// The new disposable injection scope.
        /// </returns>
        IInjectionScope CreateScope();
    }
}