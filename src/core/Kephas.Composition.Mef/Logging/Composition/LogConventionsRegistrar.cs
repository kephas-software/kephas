// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogConventionsRegistrar.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Conventions registrar for logging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Composition
{
    using System.Composition.Convention;

    using Kephas.Composition.Mef.Conventions;
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Conventions registrar for logging.
    /// </summary>
    public class LogConventionsRegistrar : MefConventionsRegistrarBase
    {
        /// <summary>
        /// The logger property name.
        /// </summary>
        public static readonly string LoggerPropertyName = ReflectionHelper.GetPropertyName<ILogConsumer>(l => l.Logger);

        /// <summary>
        /// The register conventions.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        protected override void RegisterConventions(ConventionBuilder builder)
        {
            builder.ForTypesDerivedFrom<ILogConsumer>()
              .ImportProperties(
                p => p != null && p.Name == LoggerPropertyName,
                (p, pBuilder) => pBuilder.AsContractName(TypeAffineExportDescriptorProvider.GetContractName<ILogger>(p.DeclaringType)));
        }
    }
}