// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRefPropertyInfo.cs" company="Kephas Software SRL">
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
    /// Property information indicating a reference to another entity.
    /// </summary>
    public interface IRefPropertyInfo : IPropertyInfo
    {
        /// <summary>
        /// Gets the reference type.
        /// </summary>
        ITypeInfo RefType { get; }

#if NETSTANDARD2_0
#else
        /// <summary>
        /// Gets the reference type asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding the reference type.</returns>
        Task<ITypeInfo> GetRefTypeAsync(CancellationToken cancellationToken = default) => Task.FromResult(this.RefType);
#endif
    }
}