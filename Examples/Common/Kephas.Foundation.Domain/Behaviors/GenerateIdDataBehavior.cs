// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateIdDataBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the generate identifier data behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Foundation.Domain.Behaviors
{
    using Kephas.Data;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Foundation.Domain.Abstractions;
    using Kephas.Services;

    /// <summary>
    /// A generate identifier data behavior.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class GenerateIdDataBehavior : DataBehaviorBase<IEntityBase>
    {
        /// <summary>
        /// The identifier generator.
        /// </summary>
        private readonly IIdGenerator idGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateIdDataBehavior"/> class.
        /// </summary>
        /// <param name="idGenerator">The identifier generator.</param>
        public GenerateIdDataBehavior(IIdGenerator idGenerator)
        {
            this.idGenerator = idGenerator;
        }

        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        public override void BeforePersist(IEntityBase entity, IEntityEntry entityEntry, IDataOperationContext operationContext)
        {
            if (entityEntry.ChangeState == ChangeState.Added || entityEntry.ChangeState == ChangeState.AddedOrChanged)
            {
                if (Id.IsTemporary(entityEntry.Id) || Id.IsEmpty(entityEntry.Id))
                {
                    entity.Id = this.idGenerator.GenerateId();
                }
            }
        }
    }
}