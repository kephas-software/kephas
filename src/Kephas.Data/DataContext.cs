// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System.Diagnostics.Contracts;

    using Kephas.Services;

    /// <summary>
    /// A data context.
    /// </summary>
    public class DataContext : ContextBase, IDataContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public DataContext(IDataRepository repository)
        {
            Contract.Requires(repository != null);

            this.Repository = repository;
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <value>
        /// The repository.
        /// </value>
        public IDataRepository Repository { get; }
    }
}