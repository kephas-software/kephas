// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureEventBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature event base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using System;

    using Kephas.Application.Reflection;

    /// <summary>
    /// Base class for feature events.
    /// </summary>
    public abstract class FeatureEventBase : IFeatureEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureEventBase"/> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        protected FeatureEventBase(IFeatureInfo feature)
        {
            this.Feature = feature;
        }

        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <value>
        /// The feature.
        /// </value>
        public IFeatureInfo Feature { get; }

        /// <summary>
        /// Gets the time stamp of event.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        public DateTimeOffset TimeStamp { get; } = DateTimeOffset.Now;
    }
}
