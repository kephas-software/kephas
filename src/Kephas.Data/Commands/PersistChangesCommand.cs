// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistChangesCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the persist changes command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Resources;
    using Kephas.Data.Validation;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for commands which persist dataContext changes.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class PersistChangesCommand : DataCommandBase<IPersistChangesContext, IOperationResult>, IPersistChangesCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistChangesCommand"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public PersistChangesCommand(IDataBehaviorProvider behaviorProvider, ILogManager? logManager = null)
            : base(logManager)
        {
            behaviorProvider = behaviorProvider ?? throw new System.ArgumentNullException(nameof(behaviorProvider));

            this.BehaviorProvider = behaviorProvider;
        }

        /// <summary>
        /// Gets the behavior provider.
        /// </summary>
        /// <value>
        /// The behavior provider.
        /// </value>
        public IDataBehaviorProvider BehaviorProvider { get; }

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IOperationResult"/>.
        /// </returns>
        public override async Task<IOperationResult> ExecuteAsync(
            IPersistChangesContext operationContext,
            CancellationToken cancellationToken = default)
        {
            var changes = 0;
            Exception? exception = null;
            var sb = new StringBuilder();

            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                    {
                        try
                        {
                            var currentIteration = 0;
                            const int MaxIterations = 10;

                            operationContext.Iteration = currentIteration;
                            var iterationChangeSet = this.ComputeIterationChangeSet(operationContext);
                            operationContext.IterationChangeSet = iterationChangeSet;

                            while (iterationChangeSet.Count > 0)
                            {
                                if (currentIteration > MaxIterations)
                                {
                                    var ex = new DataException(string.Format(Strings.PersistChangesCommand_MaximumNumberOfIterationsExceeded_Exception, MaxIterations));
                                    throw ex;
                                }

                                await this.ApplyBeforePersistBehaviorsAsync(operationContext, iterationChangeSet, cancellationToken).PreserveThreadContext();

                                changes = await this.ValidateAndPersistChangeSetAsync(iterationChangeSet, changes, operationContext, sb, cancellationToken).PreserveThreadContext();

                                this.AcceptChanges(operationContext, iterationChangeSet);

                                await this.ApplyAfterPersistBehaviorsAsync(operationContext, iterationChangeSet, cancellationToken).PreserveThreadContext();

                                // NOTE: after calling after save behaviors, it may happen that new changes occur, so try to save the new changes again.
                                currentIteration++;
                                operationContext.Iteration = currentIteration;
                                iterationChangeSet = this.ComputeIterationChangeSet(operationContext);
                                operationContext.IterationChangeSet = iterationChangeSet;
                            }
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                    }).PreserveThreadContext();

            // log exceptions and other important data.
            if (exception != null)
            {
                this.Logger.Error(
                    exception,
                    "{operation}: {message}, data context ID: {dataContextId}, elapsed: {elapsed}, change count: {changeCount}, iteration count: #{iterationCount}, trace: '{opTrace}'",
                    $"{nameof(PersistChangesCommand)}.{nameof(this.ExecuteAsync)}",
                    exception.Message,
                    operationContext.DataContext.Id,
                    opResult.Elapsed,
                    changes,
                    operationContext.Iteration,
                    sb);
                throw exception;
            }

            if (opResult.Elapsed.TotalMilliseconds > 1000)
            {
                this.Logger.Warn(
                    "{operation}: {message}, data context ID: {dataContextId}, elapsed: {elapsed}, change count: {changeCount}, iteration count: #{iterationCount}, trace: '{opTrace}'",
                    $"{nameof(PersistChangesCommand)}.{nameof(this.ExecuteAsync)}",
                    "Elapsed time more than 1s",
                    operationContext.DataContext.Id,
                    opResult.Elapsed,
                    changes,
                    operationContext.Iteration,
                    sb);
            }
            else
            {
                this.Logger.Debug(
                    "{operation}: {message}, data context ID: {dataContextId}, elapsed: {elapsed}, change count: {changeCount}, iteration count: #{iterationCount}, trace: '{opTrace}'",
                    $"{nameof(PersistChangesCommand)}.{nameof(this.ExecuteAsync)}",
                    "Success",
                    operationContext.DataContext.Id,
                    opResult.Elapsed,
                    changes,
                    operationContext.Iteration,
                    sb);
            }

            return new DataCommandResult(string.Format(Strings.PersistChangesCommand_ResultMessage, changes))
                .MergeAll(opResult);
        }

        /// <summary>
        /// Accepts the changes of the entities after the persistence.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="changeSet">The change set.</param>
        protected virtual void AcceptChanges(IPersistChangesContext operationContext, IList<IEntityEntry> changeSet)
        {
            var dataContext = operationContext.DataContext;
            foreach (var entityEntry in changeSet)
            {
                if (entityEntry.ChangeState != ChangeState.Deleted)
                {
                    entityEntry.AcceptChanges();
                }
                else
                {
                    dataContext.Detach(entityEntry);
                }
            }
        }

        /// <summary>
        /// Applies the behaviors invoking the <see cref="IOnPersistBehavior.AfterPersistAsync"/> asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context for persisting the changes.</param>
        /// <param name="changeSet">The modified entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task ApplyAfterPersistBehaviorsAsync(IPersistChangesContext operationContext, IList<IEntityEntry> changeSet, CancellationToken cancellationToken)
        {
            foreach (var entityEntry in changeSet)
            {
                var reversedBehaviors = this.BehaviorProvider.GetDataBehaviors<IOnPersistBehavior>(entityEntry.Entity).Reverse();
                foreach (var behavior in reversedBehaviors)
                {
                    await behavior.AfterPersistAsync(entityEntry.Entity, entityEntry, operationContext, cancellationToken).PreserveThreadContext();
                }
            }
        }

        /// <summary>
        /// Applies the behaviors invoking the <see cref="IOnPersistBehavior.BeforePersistAsync"/> asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context for persisting the changes.</param>
        /// <param name="changeSet">The modified entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task ApplyBeforePersistBehaviorsAsync(IPersistChangesContext operationContext, IList<IEntityEntry> changeSet, CancellationToken cancellationToken)
        {
            foreach (var entityEntry in changeSet)
            {
                var behaviors = this.BehaviorProvider.GetDataBehaviors<IOnPersistBehavior>(entityEntry.Entity);
                foreach (var behavior in behaviors)
                {
                    await behavior.BeforePersistAsync(entityEntry.Entity, entityEntry, operationContext, cancellationToken).PreserveThreadContext();
                }
            }
        }

        /// <summary>
        /// Computes the change set for the current iteration and returns it.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A list of modified <see cref="IEntityEntry"/> objects.
        /// </returns>
        protected virtual IList<IEntityEntry> ComputeIterationChangeSet(IPersistChangesContext operationContext)
        {
            if (operationContext.ChangeSet != null)
            {
                var dataContext = operationContext.DataContext;
                var changeSet = operationContext.ChangeSet
                                    .Select(e => dataContext.GetEntityEntry(e))
                                    .Where(ei => ei != null && ei.ChangeState != ChangeState.NotChanged)
                                    .ToList();
                return changeSet;
            }

            var localCache = this.TryGetLocalCache(operationContext.DataContext);
            if (localCache == null)
            {
                return new List<IEntityEntry>();
            }

            var changes = localCache.Values
                .Where(e => e.ChangeState != ChangeState.NotChanged)
                .ToList();

            return changes;
        }

        /// <summary>
        /// Validates and persists the modified entities.
        /// </summary>
        /// <param name="changeSet">The change set.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="sb">The string builder to append logging data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The aggregated number of changes.
        /// </returns>
        protected virtual async Task<int> ValidateAndPersistChangeSetAsync(
          IList<IEntityEntry> changeSet,
          int changes,
          IPersistChangesContext operationContext,
          StringBuilder sb,
          CancellationToken cancellationToken)
        {
            var modifiedRootEntities = changeSet
                    .Select(e => e.GetGraphRoot() ?? e.Entity)
                    .Distinct()
                    .ToList();

            if (modifiedRootEntities.Count == 0 && changeSet.Count > 0)
            {
                throw new DataException(Strings.PersistChangesCommand_NoRootEntitiesToSave_Exception);
            }

            await this.ValidateChangeSetAsync(changeSet, operationContext, cancellationToken).PreserveThreadContext();

            await this.PreProcessChangeSetAsync(changeSet, operationContext, cancellationToken).PreserveThreadContext();

            try
            {
                await this.PersistChangeSetAsync(changeSet, operationContext, cancellationToken).PreserveThreadContext();
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException ?? tie;
            }

            await this.PostProcessChangeSetAsync(changeSet, operationContext, cancellationToken).PreserveThreadContext();

            changes += changeSet.Count;
            return changes;
        }

        /// <summary>
        /// Validates the modified entries asynchronously.
        /// </summary>
        /// <param name="changeSet">The modified entities.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task ValidateChangeSetAsync(IList<IEntityEntry> changeSet, IPersistChangesContext operationContext, CancellationToken cancellationToken)
        {
            var dataContext = operationContext.DataContext;
            foreach (var entityEntry in changeSet)
            {
                var entityGraphParts = entityEntry.GetStructuralEntityGraph() ?? new[] { entityEntry.Entity };

                foreach (var entityPart in entityGraphParts)
                {
                    var entityPartInfo = entityPart == entityEntry.Entity
                                             ? entityEntry
                                             : dataContext.GetEntityEntry(entityPart);
                    if (entityPartInfo == null)
                    {
                        // TODO localization
                        this.Logger.Warn("No entity info provided for graph part '{entity}' ({entityType}), skipping validation.", entityPart, entityPart.GetType());
                    }
                    else
                    {
                        var validationErrors = await this.ValidateEntityAsync(entityPart, entityPartInfo, operationContext, cancellationToken).PreserveThreadContext();
                        if (validationErrors.HasErrors())
                        {
                            throw new DataValidationException(entityPart, validationErrors);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates the modified entity asynchronously.
        /// </summary>
        /// <param name="entityPart">The entity part.</param>
        /// <param name="entityPartEntry">Information describing the entity part.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A <see cref="Task{IDataValidationResult}"/>.
        /// </returns>
        protected virtual async Task<IDataValidationResult> ValidateEntityAsync(
            object entityPart,
            IEntityEntry entityPartEntry,
            IPersistChangesContext operationContext,
            CancellationToken cancellationToken)
        {
            var validators = this.BehaviorProvider.GetDataBehaviors<IOnValidateBehavior>(entityPart);
            var result = new DataValidationResult();
            foreach (var validator in validators)
            {
                var validatorResult = await validator.ValidateAsync(entityPart, entityPartEntry, operationContext, cancellationToken).PreserveThreadContext();
                result.Add(validatorResult.ToArray());
            }

            return result;
        }

        /// <summary>
        /// Pre processes the change set asynchronously.
        /// </summary>
        /// <remarks>
        /// By default, it sets the <see cref="IEntityEntry.PrePersistChangeState"/> to be equal to <see cref="IChangeStateTrackable.ChangeState"/>.
        /// </remarks>
        /// <param name="changeSet">The modified entities.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task PreProcessChangeSetAsync(IList<IEntityEntry> changeSet, IPersistChangesContext operationContext, CancellationToken cancellationToken)
        {
            foreach (var entityEntry in changeSet)
            {
                entityEntry.PrePersistChangeState = entityEntry.ChangeState;
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Post processes the change set asynchronously.
        /// </summary>
        /// <param name="changeSet">The modified entities.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task PostProcessChangeSetAsync(IList<IEntityEntry> changeSet, IPersistChangesContext operationContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Persists the entities in the change set asynchronously.
        /// </summary>
        /// <param name="changeSet">The modified entities.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task PersistChangeSetAsync(
            IList<IEntityEntry> changeSet,
            IPersistChangesContext operationContext,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}