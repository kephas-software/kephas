// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientCreateEntityCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the client create entity command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.ClientEntities.Commands
{
    using System.Diagnostics.Contracts;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Commands;

    /// <summary>
    /// Create entity command implementation for a <see cref="ClientDataContext"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class ClientCreateEntityCommand<T> : CreateEntityCommandBase<ClientDataContext, T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEntityCommandBase{TDataContext,T}"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        public ClientCreateEntityCommand(IDataBehaviorProvider behaviorProvider)
            : base(behaviorProvider)
        {
            Contract.Requires(behaviorProvider != null);
        }

        /// <summary>
        /// Overridable method called just before returning the result.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="result">The result.</param>
        protected override void PostCreateEntity(ICreateEntityContext operationContext, ICreateEntityResult<T> result)
        {
            var dataContext = (ClientDataContext)operationContext.DataContext;
            result.Entity = (T)dataContext.GetOrAddCacheableItem(operationContext, result.Entity, isNew: true);
        }
    }
}