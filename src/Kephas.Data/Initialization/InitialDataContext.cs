// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialDataContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the initial data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Initialization
{
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// An initial data context.
    /// </summary>
    public class InitialDataContext : Context, IInitialDataContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitialDataContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional).</param>
        /// <param name="isThreadSafe">True if is thread safe, false if not (optional).</param>
        public InitialDataContext(IAmbientServices ambientServices = null, bool isThreadSafe = false)
            : base(ambientServices, isThreadSafe)
        {
        }

        /// <summary>
        /// Gets or sets the initial data kinds.
        /// </summary>
        /// <value>
        /// The initial data kinds.
        /// </value>
        public IEnumerable<string> InitialDataKinds { get; set; }
    }
}