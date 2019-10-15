// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureStartingEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature starting event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using Kephas.Application.Reflection;

    /// <summary>
    /// Event indicating that a feature is about to start.
    /// </summary>
    public class FeatureStartingEvent : FeatureEventBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureStartingEvent"/> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        public FeatureStartingEvent(IFeatureInfo feature)
            : base(feature)
        {
        }
    }
}
