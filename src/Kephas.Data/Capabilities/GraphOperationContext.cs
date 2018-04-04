// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphOperationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the graph operation context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    /// <summary>
    /// A graph operation context.
    /// </summary>
    public class GraphOperationContext : DataOperationContext, IGraphOperationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphOperationContext"/> class.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        public GraphOperationContext(IDataContext dataContext)
            : base(dataContext)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the loose parts should be loaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if loose parts should be loaded, <c>false</c> if not.
        /// </value>
        public bool LoadLooseParts { get; set; }
    }
}