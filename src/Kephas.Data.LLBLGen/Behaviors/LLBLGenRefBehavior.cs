// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenRefBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate reference behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Data.Analysis;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.LLBLGen.Entities;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A llbl generate reference behavior.
    /// </summary>
    /// <remarks>
    /// Before persisting, it should resolve all references that are set to temporary IDs.
    /// This is the case when importing bulks of data, when the referencing is resolved over these
    /// temporary IDs.
    /// </remarks>
    [ProcessingPriority(Priority.AboveNormal)]
    public class LLBLGenRefBehavior : DataBehaviorBase<IEntityBase>
    {
        /// <summary>
        /// The reference properties provider.
        /// </summary>
        private readonly IRefPropertiesProvider refPropertiesProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenRefBehavior"/> class.
        /// </summary>
        /// <param name="refPropertiesProvider">The reference properties provider.</param>
        public LLBLGenRefBehavior(IRefPropertiesProvider refPropertiesProvider)
        {
            this.refPropertiesProvider = refPropertiesProvider;
        }

        /// <summary>Callback invoked before an entity is being persisted.</summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityEntry">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>A Task.</returns>
        public override async Task BeforePersistAsync(
            IEntityBase entity,
            IEntityEntry entityEntry,
            IDataOperationContext operationContext,
            CancellationToken cancellationToken = default)
        {
            if (entityEntry.ChangeState == ChangeState.Deleted)
            {
                return;
            }

            var refProperties = this.refPropertiesProvider.GetRefProperties(entity);

            foreach (var refProperty in refProperties)
            {
                if (Id.IsTemporary(refProperty.Id))
                {
                    await refProperty.GetAsync(throwIfNotFound: false, cancellationToken: cancellationToken).PreserveThreadContext();
                }
            }
        }
    }
}