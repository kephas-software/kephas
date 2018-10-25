// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetupContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data setup context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Setup
{
    using System.Collections.Generic;

    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// A data setup context.
    /// </summary>
    public class DataSetupContext : Context, IDataSetupContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetupContext"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context (optional).</param>
        /// <param name="isThreadSafe">True if is thread safe, false if not (optional).</param>
        public DataSetupContext(ICompositionContext compositionContext = null, bool isThreadSafe = false)
            : base(compositionContext, isThreadSafe)
        {
        }

        /// <summary>
        /// Gets or sets the data kinds.
        /// </summary>
        /// <value>
        /// The data kinds.
        /// </value>
        public IEnumerable<string> DataKinds { get; set; }
    }
}