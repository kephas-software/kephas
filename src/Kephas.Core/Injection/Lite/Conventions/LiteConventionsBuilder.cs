// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lite conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Conventions
{
    using System;
    using System.Collections.Generic;

    using Kephas.Injection.Conventions;
    using Kephas.Logging;

    /// <summary>
    /// A lightweight conventions builder.
    /// </summary>
    internal class LiteConventionsBuilder : IConventionsBuilder
    {
        public const string LiteInjectionKey = "__LiteInjection";

        private readonly IAmbientServices ambientServices;

        private readonly IList<LiteRegistrationBuilder> descriptorBuilders = new List<LiteRegistrationBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteConventionsBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public LiteConventionsBuilder(IAmbientServices ambientServices)
        {
            this.ambientServices = ambientServices;
            this.ambientServices[LiteInjectionKey] = true;
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// A <see cref="IPartBuilder"/> to further configure the rule.
        /// </returns>
        public IPartBuilder ForInstance(object instance)
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
        /// <returns>
        /// A <see cref="IPartBuilder"/> to further configure the rule.
        /// </returns>
        public IPartBuilder ForFactory(Type type, Func<IInjector, object> factory)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.ambientServices)
            {
                InstancingStrategy = factory,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new LitePartBuilder(descriptorBuilder, this.ambientServices.LogManager);
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>
        /// A <see cref="IPartBuilder"/> that must be used to specify the rule.
        /// </returns>
        public IPartBuilder ForType(Type type)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.ambientServices)
            {
                InstancingStrategy = type,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new LitePartBuilder(descriptorBuilder, this.ambientServices.LogManager);
        }

        /// <summary>
        /// Builds the container using the given parts.
        /// </summary>
        public void Build()
        {
            foreach (var descriptorBuilder in this.descriptorBuilders)
            {
                descriptorBuilder.Build();
            }
        }
    }
}
