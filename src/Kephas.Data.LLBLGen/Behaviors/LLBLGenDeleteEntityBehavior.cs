// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenDeleteEntityBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate deleted entity base behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Behaviors
{
    using System;

    using Kephas.Data;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.LLBLGen.Entities;
    using Kephas.Services;

    /// <summary>
    /// Behavior for adjusting the cache upon entity deletion.
    /// </summary>
    /// <remarks>
    /// Should be called after common behaviors.
    /// </remarks>
    [ProcessingPriority(Priority.Lowest)]
    public class LLBLGenDeleteEntityBehavior : DataBehaviorBase<IEntityBase>
    {
        /// <summary>
        /// Callback invoked after an entity has been persisted.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityEntry">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        public override void AfterPersist(IEntityBase entity, IEntityEntry entityEntry, IDataOperationContext operationContext)
        {
            var dataContext = operationContext.DataContext;
            if (entityEntry.ChangeState == ChangeState.Deleted)
            {
                // remove the deleted entity from cache so that it is not detected again as deleted.
                var cache = ((LLBLGenDataContext)dataContext).LocalCache;
                if (cache.ContainsKey(entityEntry.Id))
                {
                    throw new InvalidOperationException("The entity info was not removed from cache!");
                }
            }
        }
    }
}