﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dependency injection injector builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Extensions.DependencyInjection.Conventions;
    using Kephas.Injection;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A dependency injection injector builder.
    /// </summary>
    public class DependencyInjectionInjectorBuilder : InjectorBuilderBase<DependencyInjectionInjectorBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DependencyInjectionInjectorBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public DependencyInjectionInjectorBuilder(IInjectionBuildContext context)
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
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="parts">The parts candidating for composition.</param>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected override IInjector CreateInjectorCore(IConventionsBuilder conventions, IEnumerable<Type> parts)
        {
            var serviceProvider = conventions is IServiceProviderBuilder mediServiceProviderBuilder
                                      ? mediServiceProviderBuilder.BuildServiceProvider(parts)
                                      : conventions is IServiceCollectionProvider mediServiceCollectionProvider
                                          ? mediServiceCollectionProvider.GetServiceCollection().BuildServiceProvider()
                                          : throw new InvalidOperationException(
                                                $"The conventions instance must implement either {typeof(IServiceProviderBuilder)} or {typeof(IServiceCollectionProvider)}.");

            return new DependencyInjectionInjector(serviceProvider);
        }
    }
}