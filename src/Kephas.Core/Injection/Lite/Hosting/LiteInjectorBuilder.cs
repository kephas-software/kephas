// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lite injector builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Hosting
{
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Kephas.Injection.Lite.Conventions;

    /// <summary>
    /// A lightweight injector builder.
    /// </summary>
    public class LiteInjectorBuilder : InjectorBuilderBase<LiteInjectorBuilder>
    {
        private readonly IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteInjectorBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LiteInjectorBuilder(IInjectionBuildContext context)
            : base(context)
        {
            this.ambientServices = context.AmbientServices;
        }

        /// <summary>
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected override IInjector CreateInjectorCore(IConventionsBuilder conventions)
        {
            var liteConventions = (LiteConventionsBuilder)conventions;
            liteConventions.Build();

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
