// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateEntityResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the create entity result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    /// <summary>
    /// Encapsulates the result of a create entity command.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class CreateEntityResult<T> : DataCommandResult, ICreateEntityResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEntityResult{T}"/> class.
        /// </summary>
        /// <param name="entity">The created entity.</param>
        /// <param name="message">(Optional) The message.</param>
        /// <param name="exception">(Optional) the exception.</param>
        public CreateEntityResult(T entity, string message = null, Exception exception = null)
            : base(message, exception)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Gets or sets the created entity.
        /// </summary>
        /// <value>
        /// The created entity or <c>null</c> if no entity could be created.
        /// </value>
        public T Entity { get; set; }
    }
}