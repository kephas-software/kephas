// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteCompositionContainerBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lite composition container builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Hosting
{
    using System;
    using System.Collections.Generic;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Lite.Conventions;

    /// <summary>
    /// A lightweight composition container builder.
    /// </summary>
    public class LiteCompositionContainerBuilder : CompositionContainerBuilderBase<LiteCompositionContainerBuilder>
    {
        private IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteCompositionContainerBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LiteCompositionContainerBuilder(ICompositionRegistrationContext context)
            : base(context)
        {
            this.ambientServices = context.AmbientServices;
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
            var liteConventions = (LiteConventionsBuilder)conventions;
            liteConventions.Build(parts);

            return this.ambientServices.ToCompositionContext();
        }

        /// <summary>
        /// Factory method for creating the conventions builder.
        /// </summary>
        /// <returns>
        /// A newly created conventions builder.
        /// </returns>
        protected override IConventionsBuilder CreateConventionsBuilder()
        {
            return new LiteConventionsBuilder(this.ambientServices);
        }
    }
}
