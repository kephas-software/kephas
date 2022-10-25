// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacScopedServiceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac scoped injector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Autofac
{
    using global::Autofac;
    using Kephas.Logging;

    /// <summary>
    /// An Autofac scoped injector.
    /// </summary>
    internal class AutofacScopedServiceProvider : AutofacServiceProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacScopedServiceProvider"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="logger">The logger.</param>
        public AutofacScopedServiceProvider(IServiceProviderContainer root, ILifetimeScope scope, ILogger? logger)
            : base(root, logger)
        {
            this.Initialize(scope);
        }
    }
}