// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Expression.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the expression class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries
{
    using System.Collections.Generic;

    /// <summary>
    /// An expression.
    /// </summary>
    public class Expression
    {
        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public string Op { get; set; }

        /// <summary>
        /// Gets or sets the operation arguments.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public List<object> Args { get; set; }
    }
}