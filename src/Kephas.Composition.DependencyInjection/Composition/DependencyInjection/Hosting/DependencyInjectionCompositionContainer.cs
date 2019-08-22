// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionCompositionContainer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi composition container class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.DependencyInjection.Hosting
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A composition container for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public class DependencyInjectionCompositionContainer : DependencyInjectionCompositionContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionCompositionContainer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public DependencyInjectionCompositionContainer(ServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
    }
}