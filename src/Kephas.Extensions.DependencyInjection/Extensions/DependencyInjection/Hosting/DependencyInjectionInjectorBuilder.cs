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
    using Kephas.Injection.Builder;
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
        /// <returns>A <see cref="IRegistrationBuilder"/> that must be used to specify the rule.</returns>
        public override IRegistrationBuilder ForType(Type type)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder
            {
                InstancingStrategy = type,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new DependencyInjectionTypeRegistrationBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        public override IRegistrationBuilder ForInstance(object instance)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder
            {
                InstancingStrategy = instance,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new DependencyInjectionTypeRegistrationBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        public override IRegistrationBuilder ForFactory(Type type, Func<IServiceProvider, object> factory)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder
            {
                ContractType = type,
                InstancingStrategy = (Func<System.IServiceProvider, object>)(serviceProvider => factory(serviceProvider.GetRequiredService<IServiceProvider>())),
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new DependencyInjectionRegistrationBuilder(descriptorBuilder);
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
                this.serviceCollection.AddRange(descriptorBuilder.Build());
            }

            var serviceProvider = this.serviceCollection.BuildServiceProvider();

            return new DependencyInjectionServiceProvider(serviceProvider);
        }
    }
}