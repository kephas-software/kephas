// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Configuration
{
    using Kephas.Dynamic;

    /// <summary>
    /// Stores injection settings.
    /// </summary>
    public class InjectionSettings : Expando
    {
        /// <summary>
        /// Gets or sets the assembly file name pattern.
        /// </summary>
        /// <value>
        /// The assembly file name pattern.
        /// </value>
        public string? AssemblyFileNamePattern { get; set; }
    }
}