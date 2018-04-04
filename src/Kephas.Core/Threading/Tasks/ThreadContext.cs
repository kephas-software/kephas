// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Stores the thread context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Stores the thread context.
    /// </summary>
    public class ThreadContext : Context
    {
        /// <summary>
        /// The store actions.
        /// </summary>
        private readonly IEnumerable<Action<ThreadContext>> storeActions;

        /// <summary>
        /// The restore actions.
        /// </summary>
        private readonly IEnumerable<Action<ThreadContext>> restoreActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContext"/>
        /// class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="storeActions">The actions called when information should be stored in the context.</param>
        /// <param name="restoreActions">The actions called when information should be restored from the context.</param>
        public ThreadContext(IAmbientServices ambientServices, IEnumerable<Action<ThreadContext>> storeActions, IEnumerable<Action<ThreadContext>> restoreActions)
            : base(ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            this.storeActions = storeActions;
            this.restoreActions = restoreActions;
        }

        /// <summary>
        /// Gets or sets the current culture.
        /// </summary>
        /// <value>
        /// The current culture.
        /// </value>
        public CultureInfo CurrentCulture { get; set; }

        /// <summary>
        /// Gets or sets the current UI culture.
        /// </summary>
        /// <value>
        /// The current UI culture.
        /// </value>
        public CultureInfo CurrentUICulture { get; set; }

        /// <summary>
        /// Stores information in the thread context.
        /// </summary>
        public void Store()
        {
            if (this.storeActions == null)
            {
                return;
            }

            foreach (var storeAction in this.storeActions)
            {
                storeAction(this);
            }
        }

        /// <summary>
        /// Restores information from the thread context.
        /// </summary>
        public void Restore()
        {
            if (this.restoreActions == null)
            {
                return;
            }

            foreach (var restoreAction in this.restoreActions)
            {
                restoreAction(this);
            }
        }
    }
}