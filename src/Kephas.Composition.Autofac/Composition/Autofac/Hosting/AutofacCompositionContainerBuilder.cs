// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacCompositionContainerBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac composition container builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition.Autofac.Conventions;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Services;

    /// <summary>
    /// An Autofac composition container builder.
    /// </summary>
    public class AutofacCompositionContainerBuilder : CompositionContainerBuilderBase<AutofacCompositionContainerBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacCompositionContainerBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AutofacCompositionContainerBuilder(ICompositionRegistrationContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Factory method for creating the conventions builder.
        /// </summary>
        /// <returns>A newly created conventions builder.</returns>
        protected override IConventionsBuilder CreateConventionsBuilder()
        {
            return new AutofacConventionsBuilder();
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
            var container = conventions is IAutofacContainerBuilder autofacContainerBuilder
                                      ? autofacContainerBuilder.Build(parts)
                                      : conventions is IAutofacContainerBuilderProvider autofacContainerBuilderProvider
                                          ? autofacContainerBuilderProvider.GetContainerBuilder().Build()
                                          : throw new InvalidOperationException(
                                                $"The conventions instance must implement either {typeof(IAutofacContainerBuilder)} or {typeof(IAutofacContainerBuilderProvider)}.");

            return new AutofacCompositionContainer(container);
        }
    }
}