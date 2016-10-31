// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataRepositoryQueryable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataRepositoryQueryable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq
{
    using System.Linq;

    /// <summary>
    /// Interface for data repository queryable.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IDataRepositoryQueryable<out T> : IQueryable<T>
    {
        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        new IDataRepositoryQueryProvider Provider { get; }
    }
}