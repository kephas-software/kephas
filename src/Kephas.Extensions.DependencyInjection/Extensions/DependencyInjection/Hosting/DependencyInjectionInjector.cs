// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionInjector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Hosting
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A injector for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public class DependencyInjectionInjector : DependencyInjectionInjectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionInjector"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public DependencyInjectionInjector(ServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
    }
}