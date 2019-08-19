// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionCompositionContainerBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dependency injection composition container builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Medi.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;

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
        /// Creates a new factory export provider.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="isShared">Optional. If set to <c>true</c>, the factory returns a shared
        ///                        component, otherwise an instance component.</param>
        /// <returns>
        /// The export provider.
        /// </returns>
        protected override IExportProvider CreateFactoryExportProvider<TContract>(Func<TContract> factory, bool isShared = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new export provider based on a <see cref="T:System.IServiceProvider" />.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="isServiceRegisteredFunc">Function used to query whether the service provider
        ///                                       registers a specific service.</param>
        /// <returns>
        /// The export provider.
        /// </returns>
        protected override IExportProvider CreateServiceProviderExportProvider(IServiceProvider serviceProvider, Func<IServiceProvider, Type, bool> isServiceRegisteredFunc)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Factory method for creating the conventions builder.
        /// </summary>
        /// <returns>
        /// A newly created conventions builder.
        /// </returns>
        protected override IConventionsBuilder CreateConventionsBuilder()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}