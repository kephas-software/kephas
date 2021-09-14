// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingJobStoreContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scheduling job store context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore
{
    using System;

    using Kephas.Composition;
    using Kephas.Data;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A scheduling job store context.
    /// </summary>
    public class SchedulingJobStoreContext : Context, ISchedulingJobStoreContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingJobStoreContext"/> class.
        /// </summary>
        /// <param name="dataContextFactory">The data context factory.</param>
        /// <param name="injector">Optional. Context for the composition.</param>
        public SchedulingJobStoreContext(Func<IContext, IDataContext> dataContextFactory, IInjector injector = null)
            : base(injector)
        {
            Requires.NotNull(dataContextFactory, nameof(dataContextFactory));

            this.DataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Gets or sets the data context factory.
        /// </summary>
        /// <value>
        /// The data context factory.
        /// </value>
        public Func<IContext, IDataContext> DataContextFactory { get; set; }
    }
}