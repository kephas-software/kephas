// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefCompositionContainerBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Builder for the MEF composition container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.Composition.Hosting.Core;
    using System.Diagnostics.Contracts;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef.Conventions;
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Composition.Mef.Resources;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Runtime;

    /// <summary>
    /// Builder for the MEF composition container.
    /// </summary>
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    public class MefCompositionContainerBuilder : CompositionContainerBuilderBase<MefCompositionContainerBuilder>
    {
        /// <summary>
        /// The container configuration.
        /// </summary>
        private ContainerConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefCompositionContainerBuilder"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="platformManager">The platform manager.</param>
        public MefCompositionContainerBuilder(ILogManager logManager, IConfigurationManager configurationManager, IPlatformManager platformManager)
            : base(logManager, configurationManager, platformManager)
        {
        }

        /// <summary>
        /// Sets the composition conventions.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>This builder.</returns>
        public override MefCompositionContainerBuilder WithConventions(IConventionsBuilder conventions)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var mefConventions = conventions as IMefConventionBuilderProvider;
            if (mefConventions == null)
            {
                throw new InvalidOperationException(string.Format(Strings.InvalidConventions, typeof(IMefConventionBuilderProvider).FullName));
            }

            return base.WithConventions(conventions);
        }

        /// <summary>
        /// Sets the container configuration.
        /// </summary>
        /// <param name="containerConfiguration">The container configuration.</param>
        /// <returns>This builder.</returns>
        public MefCompositionContainerBuilder WithConfiguration(ContainerConfiguration containerConfiguration)
        {
            Contract.Requires(containerConfiguration != null);

            this.configuration = containerConfiguration;

            return this;
        }

        /// <summary>
        /// Creates a new factory provider.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="isShared">If set to <c>true</c>, the factory returns a shared component, otherwise an instance component.</param>
        /// <returns>
        /// The export provider.
        /// </returns>
        protected override IExportProvider CreateFactoryProvider<TContract>(Func<TContract> factory, bool isShared = false)
        {
            var provider = new FactoryExportDescriptorProvider<TContract>(factory, isShared);
            return provider;
        }

        /// <summary>
        /// Factory method for creating the MEF conventions builder.
        /// </summary>
        /// <returns>A newly created MEF conventions builder.</returns>
        protected override IConventionsBuilder CreateConventionsBuilder()
        {
            return new MefConventionsBuilder();
        }

        /// <summary>
        /// Creates a new composition container based on the provided conventions and assembly parts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="parts">The parts candidating for composition.</param>
        /// <returns>
        /// A new composition container.
        /// </returns>
        protected override ICompositionContext CreateContainerCore(IConventionsBuilder conventions, IEnumerable<Type> parts)
        {
            var containerConfiguration = this.configuration ?? new ContainerConfiguration();
            containerConfiguration
                .WithDefaultConventions(((IMefConventionBuilderProvider)conventions).GetConventionBuilder())
                .WithParts(parts);

            foreach (var provider in this.ExportProviders.Values)
            {
                containerConfiguration.WithProvider((ExportDescriptorProvider)provider);
            }

            return new MefCompositionContainer(containerConfiguration);
        }
    }
}