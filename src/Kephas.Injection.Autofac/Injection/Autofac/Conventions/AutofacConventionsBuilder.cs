// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Conventions
{
    using System;
    using System.Collections.Generic;
    using global::Autofac;
    using global::Autofac.Builder;
    using Kephas.Injection;
    using Kephas.Injection.Conventions;

    /// <summary>
    /// An Autofac conventions builder.
    /// </summary>
    public class AutofacConventionsBuilder : IConventionsBuilder, IAutofacContainerBuilderFactory
    {
        private readonly ContainerBuilder containerBuilder;

        private readonly IList<Action> builders = new List<Action>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacConventionsBuilder"/> class.
        /// </summary>
        public AutofacConventionsBuilder()
            : this(new ContainerBuilder())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacConventionsBuilder"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        public AutofacConventionsBuilder(ContainerBuilder containerBuilder)
        {
            this.containerBuilder = containerBuilder;
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>
        /// A <see cref="IPartConventionsBuilder" /> that must be used
        /// to specify the rule.
        /// </returns>
        public IPartConventionsBuilder ForType(Type type)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder(this.containerBuilder)
            {
                ImplementationType = type,
            };
            this.builders.Add(() => descriptorBuilder.Build());

            return new AutofacPartConventionsBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// An IPartBuilder.
        /// </returns>
        public IPartBuilder ForInstance(Type type, object instance)
        {
            this.builders.Add(() => this.containerBuilder
                .RegisterInstance(instance)
                .As(type));

            return NullPartBuilder.Instance;
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>
        /// A <see cref="IPartBuilder" /> to further configure the rule.
        /// </returns>
        public IPartBuilder ForInstanceFactory(Type type, Func<IInjector, object> factory)
        {
            var registrationBuilder = RegistrationBuilder.ForDelegate(
                    type,
                    (context, parameters) =>
                    {
                        var serviceProvider = context.Resolve<IInjector>();
                        return factory(serviceProvider);
                    });
            var partBuilder = new AutofacPartBuilder(this.containerBuilder, registrationBuilder);
            this.builders.Add(() => partBuilder.Build());

            return partBuilder;
        }

        /// <summary>
        /// Configures the container builder with the given parts.
        /// </summary>
        /// <returns>
        /// The container builder.
        /// </returns>
        public ContainerBuilder CreateContainerBuilder()
        {
            foreach (var builder in this.builders)
            {
                builder();
            }

            return this.containerBuilder;
        }

        private class NullPartBuilder : IPartBuilder
        {
            public static readonly NullPartBuilder Instance = new NullPartBuilder();

            public IPartBuilder AllowMultiple(bool value) => this;

            public IPartBuilder Scoped() => this;

            public IPartBuilder Singleton() => this;
        }
    }
}