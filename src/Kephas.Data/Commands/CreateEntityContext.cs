// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateEntityContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the create entity context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A create entity context.
    /// </summary>
    public class CreateEntityContext : DataOperationContext, ICreateEntityContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEntityContext"/> class.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <param name="entityType">Type of the entity.</param>
        public CreateEntityContext(IDataContext dataContext, Type entityType)
            : base(dataContext)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(entityType, nameof(entityType));

            this.EntityType = entityType;
        }

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; set; }
    }

    /// <summary>
    /// A create entity context.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public class CreateEntityContext<T> : CreateEntityContext, ICreateEntityContext<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEntityContext{T}"/> class.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        public CreateEntityContext(IDataContext dataContext)
            : base(dataContext, typeof(T))
        {
            Requires.NotNull(dataContext, nameof(dataContext));
        }
    }
}