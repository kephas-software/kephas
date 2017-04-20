// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryDeleteEntityCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the in memory delete entity command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Commands
{
    using Kephas.Data.Caching;
    using Kephas.Data.Commands;

    /// <summary>
    /// Delete entity command implementation for a <see cref="InMemoryDataContext"/>.
    /// </summary>
    [DataContextType(typeof(InMemoryDataContext))]
    public class InMemoryDeleteEntityCommand : DeleteEntityCommandBase
    {
        /// <summary>
        /// Tries to get the data context's local cache.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <returns>
        /// An IDataContextCache.
        /// </returns>
        protected override IDataContextCache TryGetLocalCache(IDataContext dataContext)
        {
            var inMemoryDataContext = (InMemoryDataContext)dataContext;
            return inMemoryDataContext.WorkingCache;
        }
    }
}