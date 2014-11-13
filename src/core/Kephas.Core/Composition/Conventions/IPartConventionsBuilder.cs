// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPartConventionsBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for part convention builders.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    using System;

    /// <summary>
    /// Contract for part conventions builders.
    /// </summary>
    public interface IPartConventionsBuilder
    {
        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        IPartConventionsBuilder Shared();

        /// <summary>
        /// Exports the part using the specified conventions builder.
        /// </summary>
        /// <param name="conventionsBuilder">The conventions builder.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        IPartConventionsBuilder Export(Action<IExportConventionsBuilder> conventionsBuilder = null);
    }
}