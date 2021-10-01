// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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

    using Kephas.Dynamic;

    /// <summary>
    /// Stores the thread context.
    /// </summary>
    public class ThreadContext : IDynamic
    {
        private readonly IEnumerable<Action<ThreadContext>>? storeActions;
        private readonly IEnumerable<Action<ThreadContext>>? restoreActions;
        private readonly IDictionary<string, object?> data = new Dictionary<string, object?>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContext"/>
        /// class.
        /// </summary>
        /// <param name="storeActions">The actions called when information should be stored in the context.</param>
        /// <param name="restoreActions">The actions called when information should be restored from the context.</param>
        public ThreadContext(IEnumerable<Action<ThreadContext>>? storeActions, IEnumerable<Action<ThreadContext>>? restoreActions)
        {
            this.storeActions = storeActions;
            this.restoreActions = restoreActions;
        }

        /// <summary>
        /// Gets or sets the current culture.
        /// </summary>
        /// <value>
        /// The current culture.
        /// </value>
        public CultureInfo? CurrentCulture { get; set; }

        /// <summary>
        /// Gets or sets the current UI culture.
        /// </summary>
        /// <value>
        /// The current UI culture.
        /// </value>
        public CultureInfo? CurrentUICulture { get; set; }

        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Members collection AND the strongly typed
        /// members of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="object" /> identified by the key.
        /// </value>
        /// <param name="key">The key identifying the member name.</param>
        /// <returns>The requested member value.</returns>
        public object? this[string key]
        {
            get => this.data[key];
            set => this.data[key] = value;
        }

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