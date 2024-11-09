// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureStartedEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature started event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using Kephas.Application.Reflection;

    /// <summary>
    /// Event indicating that a feature started successfully.
    /// </summary>
    public class FeatureStartedEvent : FeatureEventBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureStartedEvent"/> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        public FeatureStartedEvent(IFeatureInfo feature)
            : base(feature)
        {
        }
    }
}
