﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionServiceScope.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition service scope class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection
{
    using System;

    using Kephas.Composition;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A composition service scope.
    /// </summary>
    public class CompositionServiceScope : IServiceScope
    {
        /// <summary>
        /// Context for the scoped composition.
        /// </summary>
        private readonly IInjector scopedInjector;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionServiceScope"/> class.
        /// </summary>
        /// <param name="parentInjector">Context for the parent composition.</param>
        public CompositionServiceScope(IInjector parentInjector)
        {
            this.scopedInjector = parentInjector.CreateScopedContext();
            this.ServiceProvider = this.scopedInjector.ToServiceProvider();
        }

        /// <summary>
        /// Gets the <see cref="T:System.IServiceProvider" /> used to resolve dependencies from the scope.
        /// </summary>
        /// <value>
        /// The service provider.
        /// </value>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.scopedInjector.Dispose();
        }
    }
}