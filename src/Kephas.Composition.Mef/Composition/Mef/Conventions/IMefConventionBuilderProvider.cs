// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMefConventionBuilderProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IMefConventionBuilderProvider interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Conventions
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