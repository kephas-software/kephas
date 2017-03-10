// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryCreateEntityCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the client create entity command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Commands
{
    using System.Diagnostics.Contracts;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Commands;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Create entity command implementation for a <see cref="InMemoryDataContext"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class InMemoryCreateEntityCommand<T> : CreateEntityCommandBase<InMemoryDataContext, T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEntityCommandBase{TDataContext,T}"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        public InMemoryCreateEntityCommand(IDataBehaviorProvider behaviorProvider)
            : base(behaviorProvider)
        {
            Requires.NotNull(behaviorProvider, nameof(behaviorProvider));
        }

        /// <summary>
        /// Overridable method called just before returning the result.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="result">The result.</param>
        protected override void PostCreateEntity(ICreateEntityContext operationContext, ICreateEntityResult<T> result)
        {
            var dataContext = (InMemoryDataContext)operationContext.DataContext;
            result.Entity = (T)dataContext.GetOrAddCacheableItem(operationContext, result.Entity, isNew: true);
        }
    }
}