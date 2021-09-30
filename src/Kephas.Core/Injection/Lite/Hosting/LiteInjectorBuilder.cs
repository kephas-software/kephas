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
    using System;
    using System.Collections.Generic;

    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Kephas.Injection.Lite.Conventions;

    /// <summary>
    /// A lightweight injector builder.
    /// </summary>
    public class LiteInjectorBuilder : InjectorBuilderBase<LiteInjectorBuilder>
    {
        public const string LiteInjectionKey = "__LiteInjection";

        private readonly IAmbientServices ambientServices;

        private readonly IList<LiteRegistrationBuilder> descriptorBuilders = new List<LiteRegistrationBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteInjectorBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LiteInjectorBuilder(IInjectionBuildContext context)
            : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
            this.ambientServices = context.AmbientServices;
            this.ambientServices[LiteInjectionKey] = true;
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartBuilder"/> that must be used to specify the rule.</returns>
        public override IPartBuilder ForType(Type type)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.ambientServices)
            {
                InstancingStrategy = type,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new LitePartBuilder(descriptorBuilder, this.ambientServices.LogManager);
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        public override IPartBuilder ForInstance(object instance)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.ambientServices)
            {
                InstancingStrategy = instance,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new LitePartBuilder(descriptorBuilder, this.ambientServices.LogManager);
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        public override IPartBuilder ForFactory(Type type, Func<IInjector, object> factory)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.ambientServices)
            {
                InstancingStrategy = factory,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new LitePartBuilder(descriptorBuilder, this.ambientServices.LogManager);
        }

        /// <summary>
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected override IInjector CreateInjectorCore()
        {
            foreach (var descriptorBuilder in this.descriptorBuilders)
            {
                descriptorBuilder.Build();
            }

            return this.ambientServices.ToInjector();
        }
    }
}
