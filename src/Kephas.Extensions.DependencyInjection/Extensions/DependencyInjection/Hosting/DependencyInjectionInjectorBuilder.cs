// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dependency injection injector builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Extensions.DependencyInjection.Conventions;
    using Kephas.Injection;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A dependency injection injector builder.
    /// </summary>
    public class DependencyInjectionInjectorBuilder : InjectorBuilderBase<DependencyInjectionInjectorBuilder>
    {
        private readonly ServiceCollection serviceCollection;
        private readonly IList<ServiceDescriptorBuilder> descriptorBuilders = new List<ServiceDescriptorBuilder>();

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DependencyInjectionInjectorBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="serviceCollection">Optional. The service collection.</param>
        public DependencyInjectionInjectorBuilder(IInjectionBuildContext context, ServiceCollection? serviceCollection = null)
            : base(context)
        {
            this.serviceCollection = serviceCollection ?? new ServiceCollection();
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartBuilder"/> that must be used to specify the rule.</returns>
        public override IPartBuilder ForType(Type type)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder
            {
                InstancingStrategy = type,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new DependencyInjectionPartConventionsBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        public override IPartBuilder ForInstance(object instance)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder
            {
                InstancingStrategy = instance,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new DependencyInjectionPartConventionsBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        public override IPartBuilder ForFactory(Type type, Func<IInjector, object> factory)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder
            {
                InstancingStrategy = (Func<IServiceProvider, object>)(serviceProvider => factory(serviceProvider.GetService<IInjector>())),
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new DependencyInjectionPartBuilder(descriptorBuilder);
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
                this.serviceCollection.AddRange(descriptorBuilder.Build());
            }

            var serviceProvider = this.serviceCollection.BuildServiceProvider();

            return new DependencyInjectionInjector(serviceProvider);
        }
    }
}