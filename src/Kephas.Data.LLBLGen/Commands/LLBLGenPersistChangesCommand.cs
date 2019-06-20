// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenPersistChangesCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate persist changes command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Commands
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// A persist changes command for LLBLGen.
    /// </summary>
    [DataContextType(typeof(LLBLGenDataContext))]
    public class LLBLGenPersistChangesCommand : PersistChangesCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenPersistChangesCommand"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        public LLBLGenPersistChangesCommand(IDataBehaviorProvider behaviorProvider)
            : base(behaviorProvider)
        {
        }

        /// <summary>
        /// Persists the modified entities asynchronously.
        /// </summary>
        /// <param name="changeSet">The modified entities.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task PersistChangeSetAsync(
            IList<IEntityEntry> changeSet,
            IPersistChangesContext operationContext,
            CancellationToken cancellationToken)
        {
            var uow = new UnitOfWork2();

            foreach (var entry in changeSet)
            {
                var refetchAfterSave = entry.ChangeState == ChangeState.Added || entry.ChangeState == ChangeState.Changed || entry.ChangeState == ChangeState.AddedOrChanged;
                if (entry.ChangeState == ChangeState.Deleted)
                {
                    uow.AddForDelete((IEntity2)entry.Entity);
                }
                else
                {
                    uow.AddForSave((IEntity2)entry.Entity, null, refetchAfterSave, false);
                }
            }

            var dataAccessAdapter = ((LLBLGenDataContext)operationContext.DataContext).DataAccessAdapter;
            return uow.CommitAsync(dataAccessAdapter, cancellationToken);
        }
    }
}