// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextQueryable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataContextQueryable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq
{
    using System.Linq;

    /// <summary>
    /// Interface for a queryable object attached to a <see cref="IDataContext"/>.
    /// </summary>
    /// <typeparam name="T">The queryable item type.</typeparam>
    public interface IDataContextQueryable<out T> : IQueryable<T>
    {
        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        new IDataContextQueryProvider Provider { get; }
    }
}