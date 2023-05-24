// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelRegistryConventions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Model.Runtime
{
    using System;
    using Kephas.Services;

    /// <summary>
    /// The context for the convention model serviceRegistry.
    /// </summary>
    public class ModelRegistryConventions : Context
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelRegistryConventions"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        public ModelRegistryConventions(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Gets or sets the marker base types.
        /// </summary>
        public Type[]? MarkerBaseTypes { get; set; }

        /// <summary>
        /// Gets or sets the marker attribute types.
        /// </summary>
        public Type[]? MarkerAttributeTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include abstract classes.
        /// </summary>
        public bool IncludeAbstractClasses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include classes.
        /// </summary>
        public bool IncludeClasses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include interfaces.
        /// </summary>
        public bool IncludeInterfaces { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to exclude the marker types.
        /// </summary>
        public bool ExcludeMarkers { get; set; }
    }
}