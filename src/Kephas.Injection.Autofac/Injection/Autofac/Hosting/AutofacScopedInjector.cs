// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacScopedInjector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac scoped injector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Hosting
{
    using global::Autofac;

    /// <summary>
    /// An Autofac scoped injector.
    /// </summary>
    internal class AutofacScopedInjector : AutofacInjectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacScopedInjector"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="scope">The scope.</param>
        public AutofacScopedInjector(IInjectionContainer root, ILifetimeScope scope)
            : base(root)
        {
            this.Initialize(scope);
        }
    }
}