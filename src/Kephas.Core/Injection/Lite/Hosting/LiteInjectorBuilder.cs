// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lite composition container builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Kephas.Injection.Conventions;
using Kephas.Injection.Hosting;
using Kephas.Injection.Lite.Conventions;

namespace Kephas.Injection.Lite.Hosting
{
    /// <summary>
    /// A lightweight composition container builder.
    /// </summary>
    public class LiteInjectorBuilder : InjectorBuilderBase<LiteInjectorBuilder>
    {
        private IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteInjectorBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LiteInjectorBuilder(IInjectionRegistrationContext context)
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
        protected override IInjector CreateInjectorCore(IConventionsBuilder conventions, IEnumerable<Type> parts)
        {
            var liteConventions = (LiteConventionsBuilder)conventions;
            liteConventions.Build(parts);

            return this.ambientServices.ToInjector();
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
