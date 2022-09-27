// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetupContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data setup context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.Setup
{
    using System.Collections.Generic;
    using Kephas.Services;

    /// <summary>
    /// A data setup context.
    /// </summary>
    public class DataSetupContext : Context, IDataSetupContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetupContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="isThreadSafe">Optional. True if is thread safe, false if not.</param>
        public DataSetupContext(IServiceProvider serviceProvider, bool isThreadSafe = false)
            : base(serviceProvider, isThreadSafe)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetupContext"/> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="isThreadSafe">Optional. True if is thread safe, false if not.</param>
        public DataSetupContext(IContext parentContext, bool isThreadSafe = false)
            : base(parentContext, isThreadSafe)
        {
        }

        /// <summary>
        /// Gets or sets the data targets.
        /// </summary>
        /// <value>
        /// The data targets.
        /// </value>
        public IEnumerable<string>? Targets { get; set; }
    }
}