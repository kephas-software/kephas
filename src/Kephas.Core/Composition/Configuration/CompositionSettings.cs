// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionSettings.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the composition settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Configuration
{
    using Kephas.Dynamic;

    /// <summary>
    /// Stores composition settings.
    /// </summary>
    public class CompositionSettings : Expando
    {
        /// <summary>
        /// Gets or sets the assembly file name pattern.
        /// </summary>
        /// <value>
        /// The assembly file name pattern.
        /// </value>
        public string AssemblyFileNamePattern { get; set; }
    }
}