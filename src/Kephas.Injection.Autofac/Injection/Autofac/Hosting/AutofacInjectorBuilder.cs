// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac injector builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Hosting
{
    using System;
    using System.Collections.Generic;
    using global::Autofac;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.Autofac.Conventions;
    using Kephas.Injection.Autofac.Metadata;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;

    /// <summary>
    /// An Autofac injector builder.
    /// </summary>
    public class AutofacInjectorBuilder : InjectorBuilderBase<AutofacInjectorBuilder>
    {
        private readonly IList<Action<ContainerBuilder>> builderConfigs = new List<Action<ContainerBuilder>>();

        private ContainerBuilder? containerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacInjectorBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AutofacInjectorBuilder(IInjectionRegistrationContext context)
            : base(context)
        {
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
        /// Sets the container builder to be used for the composition.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public AutofacInjectorBuilder WithContainerBuilder(ContainerBuilder builder)
        {
            Requires.NotNull(builder, nameof(builder));

            this.containerBuilder = builder;

            return this;
        }

        /// <summary>
        /// Factory method for creating the conventions builder.
        /// </summary>
        /// <returns>A newly created conventions builder.</returns>
        protected override IConventionsBuilder CreateConventionsBuilder()
        {
            return new AutofacConventionsBuilder(this.containerBuilder ?? new ContainerBuilder());
        }

        /// <summary>
        /// Creates a new injector based on the provided conventions and assembly parts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="parts">The parts candidating for composition.</param>
        /// <returns>
        /// A new injector.
        /// </returns>
        protected override IInjector CreateInjectorCore(IConventionsBuilder conventions, IEnumerable<Type> parts)
        {
            var autofacBuilder = ((IAutofacContainerBuilderProvider)conventions).GetContainerBuilder();

            autofacBuilder.RegisterSource(new ExportFactoryRegistrationSource());
            autofacBuilder.RegisterSource(new ExportFactoryWithMetadataRegistrationSource(this.RegistrationContext.AmbientServices.TypeRegistry));

            foreach (var builderConfig in this.builderConfigs)
            {
                builderConfig(autofacBuilder);
            }

            var containerBuilder = conventions is IAutofacContainerBuilder autofacContainerBuilder
                                      ? autofacContainerBuilder.GetContainerBuilder(parts)
                                      : conventions is IAutofacContainerBuilderProvider autofacContainerBuilderProvider
                                          ? autofacContainerBuilderProvider.GetContainerBuilder()
                                          : throw new InvalidOperationException(
                                                $"The conventions instance must implement either {typeof(IAutofacContainerBuilder)} or {typeof(IAutofacContainerBuilderProvider)}.");

            return new AutofacInjector(containerBuilder);
        }
    }
}