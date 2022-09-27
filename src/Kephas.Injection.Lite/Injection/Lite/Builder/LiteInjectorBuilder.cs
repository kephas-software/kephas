// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lite injector builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Builder
{
    using System;
    using System.Collections.Generic;

    using Kephas.Injection.Builder;
    using Kephas.Injection.Lite.Internal;

    /// <summary>
    /// A lightweight injector builder.
    /// </summary>
    public class LiteInjectorBuilder : InjectorBuilderBase<LiteInjectorBuilder>
    {
        private readonly ServiceRegistry serviceRegistry = new ();

        private readonly IList<LiteRegistrationBuilder> descriptorBuilders = new List<LiteRegistrationBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteInjectorBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LiteInjectorBuilder(IInjectionBuildContext context)
            : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> that must be used to specify the rule.</returns>
        public override IRegistrationBuilder ForType(Type type)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.serviceRegistry, type);
            this.descriptorBuilders.Add(descriptorBuilder);
            return descriptorBuilder;
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        public override IRegistrationBuilder ForInstance(object instance)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.serviceRegistry, instance);
            this.descriptorBuilders.Add(descriptorBuilder);
            return descriptorBuilder;
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        public override IRegistrationBuilder ForFactory(Type type, Func<IServiceProvider, object> factory)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.serviceRegistry, factory);
            this.descriptorBuilders.Add(descriptorBuilder);
            descriptorBuilder.As(type);
            return descriptorBuilder;
        }

        /// <summary>
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected override IServiceProvider CreateInjectorCore()
        {
            foreach (var descriptorBuilder in this.descriptorBuilders)
            {
                descriptorBuilder.Build();
            }

            return this.serviceRegistry.ToInjector();
        }
    }
}
