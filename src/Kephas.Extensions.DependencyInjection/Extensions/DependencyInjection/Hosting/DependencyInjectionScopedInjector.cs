// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionScopedInjector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Hosting
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A scoped injector for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public class DependencyInjectionScopedInjector : DependencyInjectionInjectorBase
    {
        private readonly IServiceScope serviceScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionScopedInjector"/> class.
        /// </summary>
        /// <param name="serviceScope">The service scope.</param>
        public DependencyInjectionScopedInjector(IServiceScope serviceScope)
            : base(serviceScope.ServiceProvider)
        {
            this.serviceScope = serviceScope;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public override void Dispose()
        {
            this.serviceScope.Dispose();
        }
    }
}