// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITrigger interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Triggers
{
    using System;

    using Kephas.Data;
    using Kephas.Dynamic;
    using Kephas.Scheduling.Reflection;

    /// <summary>
    /// Interface for trigger.
    /// </summary>
    public interface ITrigger : IExpando, IInstance<ITriggerInfo>, IIdentifiable, IDisposable
    {
        /// <summary>
        /// Occurs when the trigger is fired.
        /// </summary>
        event EventHandler Fire;

        /// <summary>
        /// Occurs when the trigger reached its end of life.
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Gets or sets a value indicating whether the trigger is enabled.
        /// </summary>
        bool IsEnabled { get; set; }
    }
}