// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dependency injection conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Conventions
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.Conventions;
    using Kephas.Logging;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A conventions builder for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public class ConventionsBuilder : Loggable, IConventionsBuilder, IServiceCollectionProvider, IServiceProviderBuilder
    {
        private readonly IServiceCollection serviceCollection;

        private readonly IList<ServiceDescriptorBuilder> descriptorBuilders = new List<ServiceDescriptorBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionsBuilder"/> class.
        /// </summary>
        public ConventionsBuilder()
            : this(new ServiceCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionsBuilder"/> class.
        /// </summary>
        /// <param name="serviceCollection">Collection of services.</param>
        public ConventionsBuilder(IServiceCollection serviceCollection)
        {
            Requires.NotNull(serviceCollection, nameof(serviceCollection));

            this.serviceCollection = serviceCollection;
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
            var descriptorBuilder = new ServiceDescriptorBuilder
                                        {
                                            ImplementationType = type,
                                        };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new DependencyInjectionPartConventionsBuilder(descriptorBuilder);
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
            var descriptorBuilder = new ServiceDescriptorBuilder
                                     {
                                         ServiceType = type,
                                         Instance = instance,
                                     };
            this.descriptorBuilders.Add(descriptorBuilder);
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
            var descriptorBuilder = new ServiceDescriptorBuilder
                                        {
                                            ServiceType = type,
                                            Factory = serviceProvider => factory(serviceProvider.GetService<IInjector>()),
                                        };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new DependencyInjectionPartBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Gets service collection.
        /// </summary>
        /// <returns>
        /// The service collection.
        /// </returns>
        public IServiceCollection GetServiceCollection() => this.serviceCollection;

        /// <summary>
        /// Builds the service provider.
        /// </summary>
        /// <returns>
        /// A ServiceProvider.
        /// </returns>
        public ServiceProvider BuildServiceProvider()
        {
            foreach (var descriptorBuilder in this.descriptorBuilders)
            {
                this.serviceCollection.AddRange(descriptorBuilder.Build());
            }

            return this.serviceCollection.BuildServiceProvider();
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