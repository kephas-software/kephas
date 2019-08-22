// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Conventions
{
    using System;
    using System.Collections.Generic;

    using global::Autofac;
    using global::Autofac.Builder;

    using Kephas.Composition.Conventions;

    /// <summary>
    /// An Autofac conventions builder.
    /// </summary>
    public class AutofacConventionsBuilder : IConventionsBuilder, IAutofacContainerBuilderProvider, IAutofacContainerBuilder
    {
        private readonly ContainerBuilder containerBuilder;

        private readonly IList<ServiceDescriptorBuilder> descriptorBuilders = new List<ServiceDescriptorBuilder>();

        private readonly IList<AutofacPartBuilder> factoryBuilders = new List<AutofacPartBuilder>();

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
        /// Define a rule that will apply to all types that derive from (or implement) the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>
        /// A <see cref="T:Kephas.Composition.Conventions.IPartConventionsBuilder" /> that must be used
        /// to specify the rule.
        /// </returns>
        public IPartConventionsBuilder ForTypesDerivedFrom(Type type)
        {
            return this.ForTypesMatching(t => t.IsClass && !t.IsAbstract && !ReferenceEquals(type, t) && type.IsAssignableFrom(t));
        }

        /// <summary>
        /// Define a rule that will apply to all types that derive from (or implement) the specified type.
        /// </summary>
        /// <param name="typePredicate">The type predicate.</param>
        /// <returns>
        /// A <see cref="T:Kephas.Composition.Conventions.IPartConventionsBuilder" /> that must be used
        /// to specify the rule.
        /// </returns>
        public IPartConventionsBuilder ForTypesMatching(Predicate<Type> typePredicate)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder(this.containerBuilder)
            {
                ImplementationTypePredicate = typePredicate,
            };
            this.descriptorBuilders.Add(descriptorBuilder);

            return new AutofacPartConventionsBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>
        /// A <see cref="T:Kephas.Composition.Conventions.IPartConventionsBuilder" /> that must be used
        /// to specify the rule.
        /// </returns>
        public IPartConventionsBuilder ForType(Type type)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder(this.containerBuilder)
            {
                ImplementationType = type,
            };
            this.descriptorBuilders.Add(descriptorBuilder);

            return new AutofacPartConventionsBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="instance">The instance.</param>
        public void ForInstance(Type type, object instance)
        {
            this.containerBuilder
                .RegisterInstance(instance)
                .As(type);
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>
        /// A <see cref="T:Kephas.Composition.Conventions.IPartBuilder" /> to further configure the rule.
        /// </returns>
        public IPartBuilder ForInstanceFactory(Type type, Func<ICompositionContext, object> factory)
        {
            var registrationBuilder = RegistrationBuilder.ForDelegate(
                    type,
                    (context, parameters) =>
                    {
                        var serviceProvider = context.Resolve<ICompositionContext>();
                        return factory(serviceProvider);
                    });
            var partBuilder = new AutofacPartBuilder(this.containerBuilder, registrationBuilder);
            this.factoryBuilders.Add(partBuilder);

            return partBuilder;
        }

        /// <summary>
        /// Gets the container builder.
        /// </summary>
        /// <returns>
        /// The container builder.
        /// </returns>
        public ContainerBuilder GetContainerBuilder() => this.containerBuilder;

        /// <summary>
        /// Configures the container builder with the given parts.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// The container builder.
        /// </returns>
        public ContainerBuilder GetContainerBuilder(IEnumerable<Type> parts)
        {
            foreach (var descriptorBuilder in this.descriptorBuilders)
            {
                descriptorBuilder.Build(parts);
            }

            foreach (var factoryBuilder in this.factoryBuilders)
            {
                factoryBuilder.Build(parts);
            }

            return this.containerBuilder;
        }
    }
}