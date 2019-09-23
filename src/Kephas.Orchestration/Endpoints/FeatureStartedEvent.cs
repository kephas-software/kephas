// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureStartedEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature started event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    using System;

    using Kephas.Application;

    /// <summary>
    /// Event indicating that a feature started successfully.
    /// </summary>
    public class FeatureStartedEvent : IFeatureEvent
    {
        /// <summary>
        /// Gets or sets the feature.
        /// </summary>
        /// <value>
        /// The feature.
        /// </value>
        public IFeatureInfo Feature { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
    }
}
