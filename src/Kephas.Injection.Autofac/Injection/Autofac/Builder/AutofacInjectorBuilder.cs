// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac injector builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Builder
{
    using System;
    using System.Collections.Generic;

    using global::Autofac;
    using global::Autofac.Builder;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.Autofac.Metadata;
    using Kephas.Injection.Builder;

    /// <summary>
    /// An Autofac injector builder.
    /// </summary>
    public class AutofacInjectorBuilder : InjectorBuilderBase<AutofacInjectorBuilder>
    {
        private readonly ContainerBuilder containerBuilder;
        private readonly IList<Action<ContainerBuilder>> builderConfigs = new List<Action<ContainerBuilder>>();
        private readonly IList<IAutofacRegistrationBuilder> partBuilders = new List<IAutofacRegistrationBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacInjectorBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="containerBuilder">Optional. The container builder.</param>
        public AutofacInjectorBuilder(IInjectionBuildContext context, ContainerBuilder? containerBuilder = null)
            : base(context)
        {
            this.containerBuilder = containerBuilder ?? new ContainerBuilder();
        }

        /// <summary>
        /// Adds a builder configuration action.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public AutofacInjectorBuilder WithConfig(Action<ContainerBuilder> config)
        {
            Requires.NotNull(config, nameof(config));

            this.builderConfigs.Add(config);

            return this;
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> that must be used to specify the rule.</returns>
        public override IRegistrationBuilder ForType(Type type)
        {
            var registrationBuilder = new ServiceDescriptorBuilder(this.containerBuilder)
            {
                ImplementationType = type,
            };

            var partBuilder = new AutofacTypeRegistrationBuilder(registrationBuilder);
            this.partBuilders.Add(partBuilder);

            return partBuilder;
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        public override IRegistrationBuilder ForInstance(object instance)
        {
            var registrationBuilder = this.containerBuilder
                .RegisterInstance(instance);
            var partBuilder = new AutofacSimpleRegistrationBuilder(this.containerBuilder, registrationBuilder, isRegistered: true);
            this.partBuilders.Add(partBuilder);

            return partBuilder;
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
        public override IRegistrationBuilder ForFactory(Type type, Func<IInjector, object> factory)
        {
            var registrationBuilder = RegistrationBuilder.ForDelegate(
                type,
                (context, parameters) =>
                {
                    var serviceProvider = context.Resolve<IInjector>();
                    return factory(serviceProvider);
                });
            var partBuilder = new AutofacSimpleRegistrationBuilder(this.containerBuilder, registrationBuilder, isRegistered: false);
            partBuilder.As(type);
            this.partBuilders.Add(partBuilder);

            return partBuilder;
        }

        /// <summary>
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected override IInjector CreateInjectorCore()
        {
            foreach (var partBuilder in this.partBuilders)
            {
                partBuilder.Build();
            }

            this.containerBuilder.RegisterSource(new ExportFactoryRegistrationSource());
            this.containerBuilder.RegisterSource(new ExportFactoryWithMetadataRegistrationSource());

            foreach (var builderConfig in this.builderConfigs)
            {
                builderConfig(this.containerBuilder);
            }

            return new AutofacInjector(this.containerBuilder);
        }
    }
}