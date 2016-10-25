// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistChangesContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the persist changes context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A context for persisting changes.
    /// </summary>
    public class PersistChangesContext : DataContext, IPersistChangesContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistChangesContext"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public PersistChangesContext(IDataRepository repository)
            : base(repository)
        {
            Contract.Requires(repository != null);
        }
    }
}