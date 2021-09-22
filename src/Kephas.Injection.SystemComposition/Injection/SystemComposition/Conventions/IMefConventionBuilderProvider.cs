// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMefConventionBuilderProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMefConventionBuilderProvider interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Conventions
{
    using System.Composition.Convention;

    /// <summary>
    /// Provider for <see cref="ConventionBuilder"/>.
    /// </summary>
    public interface IMefConventionBuilderProvider
    {
        /// <summary>
        /// Gets the convention builder.
        /// </summary>
        /// <returns>The convention builder.</returns>
        ConventionBuilder GetConventionBuilder();
    }
}