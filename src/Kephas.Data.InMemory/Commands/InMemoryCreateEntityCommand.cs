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
    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Create entity command implementation for a <see cref="InMemoryDataContext"/>.
    /// </summary>
    [DataContextType(typeof(InMemoryDataContext))]
    public class InMemoryCreateEntityCommand : CreateEntityCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCreateEntityCommand"/> class.
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
        protected override void PostCreateEntity(ICreateEntityContext operationContext, ICreateEntityResult result)
        {
            var dataContext = (InMemoryDataContext)operationContext.DataContext;
            result.Entity = dataContext.GetOrAddCacheableItem(operationContext, new EntityInfo(result.Entity, ChangeState.Added));
        }
    }
}