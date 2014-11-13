// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for MEF 2.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef
{
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.Metadata;
    using Kephas.Services;

    /// <summary>
    /// Extension methods for MEF 2.
    /// </summary>
    public static class CompositionExtensions
    {
        /// <summary>
        /// Adds the processing priority order metadata.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The provided <see cref="ExportConventionBuilder"/>.</returns>
        public static ExportConventionBuilder AddProcessingPriorityMetadata(this ExportConventionBuilder builder)
        {
            Contract.Requires(builder != null);

            builder.AddMetadata(AppServiceMetadata.ProcessingPriorityKey, t => t.ExtractMetadataValue<ProcessingPriorityAttribute, int>());

            return builder;
        }

        /// <summary>
        /// Add conventions defined in the provided convention assemblies. These will be used as the default conventions; types and assemblies added with a specific convention will use their own.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="conventionAssemblies">The convention assemblies.</param>
        /// <returns>A configuration object allowing configuration to continue.</returns>
        public static ContainerConfiguration WithDefaultConventionsFrom(this ContainerConfiguration configuration, params Assembly[] conventionAssemblies)
        {
            return WithDefaultConventionsFrom(configuration, (IEnumerable<Assembly>)conventionAssemblies);
        }

        /// <summary>
        /// Add conventions defined in the provided convention assemblies. These will be used as the default conventions; types and assemblies added with a specific convention will use their own.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="conventionAssemblies">The convention assemblies.</param>
        /// <returns>A configuration object allowing configuration to continue.</returns>
        public static ContainerConfiguration WithDefaultConventionsFrom(this ContainerConfiguration configuration, IEnumerable<Assembly> conventionAssemblies)
        {
            Contract.Requires(configuration != null);

            var conventions = new MefConventionsBuilder();
            conventions.WithConventionsFrom(conventionAssemblies);
            configuration.WithDefaultConventions(conventions.GetConventionBuilder());

            return configuration;
        }
    }
}