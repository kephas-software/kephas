// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextQueryable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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