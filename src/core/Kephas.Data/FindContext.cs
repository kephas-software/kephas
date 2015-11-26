// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of a find context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Services;

    /// <summary>
    /// The default implementation of a find context.
    /// </summary>
    public class FindContext : ContextBase, IFindContext
    {
        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception if an entity is not found.
        /// </summary>
        /// <value>
        /// <c>true</c>true to throw an exception if an entity is not found, otherwise <c>false</c>.
        /// </value>
        public bool ThrowIfNotFound { get; set; }
    }
}