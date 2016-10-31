// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistChangesCommandBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the persist changes command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Resources;
    using Kephas.Data.Validation;
    using Kephas.Diagnostics;
    using Kephas.Logging;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for commands which persist repository changes.
    /// </summary>
    public abstract class PersistChangesCommandBase : DataCommandBase<IPersistChangesContext, IDataCommandResult>,
                                                      IPersistChangesCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistChangesCommandBase"/> class.
        /// </summary>
        /// <param name="dataValidationService">The validation service.</param>
        protected PersistChangesCommandBase(IDataValidationService dataValidationService)
        {
            Contract.Requires(dataValidationService != null);

            this.DataValidationService = dataValidationService;
        }

        /// <summary>
        /// Gets the validation service.
        /// </summary>
        /// <value>
        /// The validation service.
        /// </value>
        public IDataValidationService DataValidationService { get; }


        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<PersistChangesCommandBase> Logger { get; set; }

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public override async Task<IDataCommandResult> ExecuteAsync(
            IPersistChangesContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var changes = 0;
            Exception exception = null;
            var sb = new StringBuilder();

            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                    {
                        try
                        {
                            var currentIteration = 0;
                            const int MaxIterations = 10;

                            var modifiedEntries = await this.DetectModifiedEntriesAsync(context, cancellationToken).PreserveThreadContext();

                            while (modifiedEntries.Count > 0)
                            {
                                if (currentIteration > MaxIterations)
                                {
                                    var ex = new DataException(string.Format(Strings.PersistChangesCommand_MaximumNumberOfIterationsExceeded_Exception, MaxIterations));
                                    throw ex;
                                }

                                await this.ExecuteBeforeSaveBehaviorsAsync(context, modifiedEntries, cancellationToken).PreserveThreadContext();

                                changes = await this.ValidateAndPersistModifiedEntriesAsync(modifiedEntries, changes, context, sb, cancellationToken).PreserveThreadContext();

                                await this.ExecuteAfterSaveBehaviorsAsync(context, modifiedEntries, cancellationToken).PreserveThreadContext();

                                // NOTE: after calling after save behaviors, it may happen that new changes occur, so try to save the new changes again.
                                modifiedEntries = await this.DetectModifiedEntriesAsync(context, cancellationToken).PreserveThreadContext();
                                currentIteration++;
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
                        "PersistChangesCommand.ExecuteAsync|ID: {0}|Message: {1}|Elapsed: {2}|Change count: {3}|Data: {4}",
                        context.Repository.Id,
                        exception.Message,
                        elapsed,
                        changes,
                        sb);
                throw exception;
            }

            if (elapsed.TotalMilliseconds > 1000)
            {
                this.Logger.Warn(
                        "PersistChangesCommand.ExecuteAsync|ID: {0}|Message: {1}|Elapsed: {2}|Change count: {3}|Data: {4}",
                        context.Repository.Id,
                        "Elapsed time more than 1s",
                        elapsed,
                        changes,
                        sb);
            }
            else
            {
                this.Logger.Debug(
                    "PersistChangesCommand.ExecuteAsync|ID: {0}|Message: {1}|Elapsed: {2}|Change count: {3}|Data: {4}",
                    context.Repository.Id,
                    "OK",
                    elapsed,
                    changes,
                    sb);
            }

            return new DataCommandResult(string.Format(Strings.PersistChangesCommand_ResultMessage, changes));
        }

        /// <summary>
        /// Executes the after save behaviors asynchronously.
        /// </summary>
        /// <param name="context">The context for persisting the changes.</param>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task ExecuteAfterSaveBehaviorsAsync(IPersistChangesContext context, IList<IPersistChangesEntry> modifiedEntries, CancellationToken cancellationToken)
        {
            foreach (var entry in modifiedEntries)
            {
                var persistableBehavior = context.Repository.TryGetCapability<IAsyncPersistable>(entry.ModifiedEntity, context);
                if (persistableBehavior != null)
                {
                    await persistableBehavior.AfterPersistAsync(context, cancellationToken).PreserveThreadContext();
                }
            }
        }

        /// <summary>
        /// Executes the before save behaviors asynchronously.
        /// </summary>
        /// <param name="context">The context for persisting the changes.</param>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task ExecuteBeforeSaveBehaviorsAsync(IPersistChangesContext context, IList<IPersistChangesEntry> modifiedEntries, CancellationToken cancellationToken)
        {
            foreach (var entry in modifiedEntries)
            {
                var persistableBehavior = context.Repository.TryGetCapability<IAsyncPersistable>(entry.ModifiedEntity, context);
                if (persistableBehavior != null)
                {
                    await persistableBehavior.BeforePersistAsync(context, cancellationToken).PreserveThreadContext();
                }
            }
        }

        /// <summary>
        /// Detects the modified entries and returns them.
        /// </summary>
        /// <param name="context">The entity context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A list of modified entries tuples.
        /// </returns>
        protected abstract Task<IList<IPersistChangesEntry>> DetectModifiedEntriesAsync(IPersistChangesContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Saves the modified entries.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="entityContext">The entity context.</param>
        /// <param name="sb">The string builder to append logging data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The aggregated number of changes.
        /// </returns>
        protected virtual async Task<int> ValidateAndPersistModifiedEntriesAsync(
          IList<IPersistChangesEntry> modifiedEntries,
          int changes,
          IPersistChangesContext entityContext,
          StringBuilder sb,
          CancellationToken cancellationToken)
        {
            var modifiedRootEntities =
              modifiedEntries.Select(e => entityContext.Repository.TryGetCapability<IAsyncAggregatable>(e.ModifiedEntity, entityContext)?.GetGraphRoot())
                .Distinct()
                .ToList();

            if (modifiedRootEntities.Count == 0 && modifiedEntries.Count > 0)
            {
                throw new DataException(Strings.PersistChangesCommand_NoRootEntitiesToSave_Exception);
            }

            await this.ValidateModifiedEntriesAsync(modifiedEntries, entityContext, cancellationToken).PreserveThreadContext();

            await this.PreProcessModifiedEntriesAsync(modifiedEntries, entityContext, cancellationToken).PreserveThreadContext();

            try
            {
                await this.PersistModifiedEntriesAsync(modifiedEntries, entityContext, cancellationToken).PreserveThreadContext();
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }

            await this.PostProcessModifiedEntriesAsync(modifiedEntries, entityContext, cancellationToken).PreserveThreadContext();

            changes += modifiedEntries.Count;
            return changes;
        }

        /// <summary>
        /// Validates the modified entries asynchronously.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="entityContext">The entity context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task ValidateModifiedEntriesAsync(IList<IPersistChangesEntry> modifiedEntries, IPersistChangesContext entityContext, CancellationToken cancellationToken)
        {
            foreach (var entry in modifiedEntries)
            {
                foreach (var entityPart in entry.FlattenedEntityGraph)
                {
                    var validationErrors = await this.DataValidationService.ValidateAsync(entityPart, entityContext, cancellationToken).PreserveThreadContext();
                    if (validationErrors.HasErrors())
                    {
                        throw new DataValidationException(entityPart, validationErrors);
                    }
                }
            }
        }

        /// <summary>
        /// Pre processes the modified entries asynchronously.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="entityContext">The entity context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task PreProcessModifiedEntriesAsync(IList<IPersistChangesEntry> modifiedEntries, IPersistChangesContext entityContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Post processes the modified entries asynchronously.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="entityContext">The entity context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task PostProcessModifiedEntriesAsync(IList<IPersistChangesEntry> modifiedEntries, IPersistChangesContext entityContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Persists the modified entries asynchronously.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="entityContext">The entity context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected abstract Task PersistModifiedEntriesAsync(
            IList<IPersistChangesEntry> modifiedEntries,
            IPersistChangesContext entityContext,
            CancellationToken cancellationToken);
    }
}