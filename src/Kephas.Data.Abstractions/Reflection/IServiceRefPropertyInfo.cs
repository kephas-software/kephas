// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRefPropertyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Reflection
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Reflection;

    /// <summary>
    /// Property information indicating a reference to a service.
    /// </summary>
    public interface IServiceRefPropertyInfo : IPropertyInfo
    {
        /// <summary>
        /// Gets the service reference type.
        /// </summary>
        ITypeInfo ServiceRefType { get; }

        /// <summary>
        /// Gets the service reference type asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding the service reference type.</returns>
        Task<ITypeInfo> GetServiceRefTypeAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(this.ServiceRefType);
    }
}