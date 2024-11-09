// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureStoppedEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature stopped event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using Kephas.Application.Reflection;

    /// <summary>
    /// Event indicating that a feature was stopped.
    /// </summary>
    public class FeatureStoppedEvent : FeatureEventBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureStoppedEvent"/> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        public FeatureStoppedEvent(IFeatureInfo feature)
            : base(feature)
        {
        }
    }
}
