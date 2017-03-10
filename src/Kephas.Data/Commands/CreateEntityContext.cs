// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateEntityContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the create entity context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A create entity context.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public class CreateEntityContext<T> : DataOperationContext, ICreateEntityContext<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEntityContext{T}"/> class.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        public CreateEntityContext(IDataContext dataContext)
            : base(dataContext)
        {
            Requires.NotNull(dataContext, nameof(dataContext));

            this.EntityType = typeof(T);
        }

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; set; }
    }
}