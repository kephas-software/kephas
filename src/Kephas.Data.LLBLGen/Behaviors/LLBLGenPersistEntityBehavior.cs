// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenPersistEntityBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the common entity base behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Behaviors
{
    using System.Collections;
    using System.Linq;

    using Kephas.Data;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.LLBLGen.Entities;
    using Kephas.Services;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// Behavior for preparing entities for persistence.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class LLBLGenPersistEntityBehavior : DataBehaviorBase<IEntityBase>
    {
        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityEntry">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        public override void BeforePersist(IEntityBase entity, IEntityEntry entityEntry, IDataOperationContext operationContext)
        {
            var changeState = entityEntry.ChangeState;
            if (changeState == ChangeState.Deleted)
            {
                return;
            }

            // Normally, it shouldn't be possible to have a non-authorized call here.
            PrepareEntityForSave(entity, false, changeState == ChangeState.Added);
        }

        /// <summary>
        /// Corrects the entity state by setting the entity to new or modified.
        /// </summary>
        /// <param name="entity">The entity with a potentially broken entity state.</param>
        /// <param name="isRecursive">Prepare nested entities.</param>
        /// <param name="isNew">True if this object is new.</param>
        private static void PrepareEntityForSave(IEntity2 entity, bool isRecursive, bool isNew)
        {
            if (!isRecursive)
            {
                return;
            }

            // iterate the list of nested entities by using reflection when recursion is demanded
            var entityType = entity.GetType();
            var entityProperties = entityType.GetProperties();
            var collectionProperties =
                entityProperties.Where(
                    property =>
                        property.PropertyType.IsGenericType &&
                        property.PropertyType.GetGenericTypeDefinition().Name.StartsWith("EntityCollection"));
            foreach (var collectionProperty in collectionProperties)
            {
                var entities = (IEnumerable)collectionProperty.GetValue(entity);
                foreach (IEntity2 commonEntityBase in entities)
                {
                    PrepareEntityForSave(commonEntityBase, isNew, true);
                }
            }
        }
    }
}