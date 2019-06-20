// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenQueryProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate query provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using System;
    using System.Linq;

    using Kephas.Data;
    using Kephas.Data.Linq;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// A llbl generate query provider.
    /// </summary>
    public class LLBLGenQueryProvider : DataContextQueryProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenQueryProvider" /> class.
        /// </summary>
        /// <param name="queryOperationContext">The query operation context.</param>
        /// <param name="nativeQueryProvider">The native query provider.</param>
        public LLBLGenQueryProvider(IQueryOperationContext queryOperationContext, IQueryProvider nativeQueryProvider)
            : base(queryOperationContext, nativeQueryProvider)
        {
        }

        /// <summary>Indicates whether an entity is attachable.</summary>
        /// <param name="entity">The entity.</param>
        /// <returns>True if the entity is attachable, false if not.</returns>
        protected override bool IsAttachable(object entity)
        {
            return entity is IEntityCore;
        }

        /// <summary>Indicates whether an entity type is attachable.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>True if the entity type is attachable, false if not.</returns>
        protected override bool IsAttachableType(Type entityType)
        {
            return typeof(IEntityCore).IsAssignableFrom(entityType);
        }
    }
}