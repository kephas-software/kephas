// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSourceContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data source context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.DataSources
{
    using Kephas.Reflection;

    /// <summary>
    /// A data source context.
    /// </summary>
    public class DataSourceContext : DataOperationContext, IDataSourceContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="projectedEntityType">Type of the projected entity.</param>
        public DataSourceContext(IDataContext dataContext, ITypeInfo entityType, ITypeInfo projectedEntityType)
            : base(dataContext)
        {
            this.EntityType = entityType;
            this.ProjectedEntityType = projectedEntityType;
        }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public ITypeInfo EntityType { get; }

        /// <summary>
        /// Gets the type of the projected entity.
        /// </summary>
        /// <value>
        /// The type of the projected entity.
        /// </value>
        public ITypeInfo ProjectedEntityType { get; }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public object Options { get; set; }
    }
}