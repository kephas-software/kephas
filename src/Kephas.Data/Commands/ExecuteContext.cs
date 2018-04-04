// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the execute context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    /// <summary>
    /// An execute context.
    /// </summary>
    public class ExecuteContext : DataOperationContext, IExecuteContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public ExecuteContext(IDataContext dataContext)
            : base(dataContext)
        {
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        public string CommandText { get; set; }
    }
}