// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureStoppingEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature stopping event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using Kephas.Application.Reflection;

    /// <summary>
    /// Event indicating that a feature is about to be stopped.
    /// </summary>
    public class FeatureStoppingEvent : FeatureEventBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureStoppingEvent"/> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        public FeatureStoppingEvent(IFeatureInfo feature)
            : base(feature)
        {
        }
    }
}
