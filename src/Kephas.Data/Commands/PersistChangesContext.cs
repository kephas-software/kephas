// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistChangesContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the persist changes operationContext class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Diagnostics.Contracts;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A operationContext for persisting changes.
    /// </summary>
    public class PersistChangesContext : DataOperationContext, IPersistChangesContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistChangesContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public PersistChangesContext(IDataContext dataContext)
            : base(dataContext)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
        }
    }
}