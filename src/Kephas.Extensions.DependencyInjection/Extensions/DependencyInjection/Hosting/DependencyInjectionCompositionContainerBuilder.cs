﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionCompositionContainerBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dependency injection composition container builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition.Conventions;
    using Kephas.Extensions.DependencyInjection.Conventions;
    using Kephas.Composition.Hosting;

    using Microsoft.Extensions.DependencyInjection;
    using Kephas.Composition;

    /// <summary>
    /// A dependency injection composition container builder.
    /// </summary>
    public class DependencyInjectionCompositionContainerBuilder : CompositionContainerBuilderBase<DependencyInjectionCompositionContainerBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DependencyInjectionCompositionContainerBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public DependencyInjectionCompositionContainerBuilder(ICompositionRegistrationContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Factory method for creating the conventions builder.
        /// </summary>
        /// <returns>
        /// A newly created conventions builder.
        /// </returns>
        protected override IConventionsBuilder CreateConventionsBuilder()
        {
            return new ConventionsBuilder();
        }

        /// <summary>
        /// Creates a new composition container based on the provided conventions and assembly parts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="parts">The parts candidating for composition.</param>
        /// <returns>
        /// A new composition container.
        /// </returns>
        protected override ICompositionContext CreateContainerCore(IConventionsBuilder conventions, IEnumerable<Type> parts)
        {
            var serviceProvider = conventions is IServiceProviderBuilder mediServiceProviderBuilder
                                      ? mediServiceProviderBuilder.BuildServiceProvider(parts)
                                      : conventions is IServiceCollectionProvider mediServiceCollectionProvider
                                          ? mediServiceCollectionProvider.GetServiceCollection().BuildServiceProvider()
                                          : throw new InvalidOperationException(
                                                $"The conventions instance must implement either {typeof(IServiceProviderBuilder)} or {typeof(IServiceCollectionProvider)}.");

            return new DependencyInjectionCompositionContainer(serviceProvider);
        }
    }
}