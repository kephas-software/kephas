// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFeatureEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFeatureEvent interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using System;

    using Kephas.Application.Reflection;

    /// <summary>
    /// Interface for feature event.
    /// </summary>
    public interface IFeatureEvent
    {
        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <value>
        /// The feature.
        /// </value>
        IFeatureInfo Feature { get; }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        DateTimeOffset TimeStamp { get; }
    }
}
