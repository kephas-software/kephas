// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryExportDescriptorProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The export factory export descriptor provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ExportProviders
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition.Mef.Internals;
    using Kephas.Reflection;

    /// <summary>
    /// The export factory export descriptor provider.
    /// </summary>
    public class ExportFactoryExportDescriptorProvider : ExportDescriptorProvider, IExportProvider
    {
        /// <summary>
        /// The get export factory definitions method.
        /// </summary>
        private static readonly MethodInfo GetExportFactoryDefinitionsMethod = typeof(ExportFactoryExportDescriptorProvider).GetTypeInfo()
                                                                                    .GetDeclaredMethod("GetExportFactoryDescriptors");

        /// <summary>
        /// Gets the export factory descriptors.
        /// </summary>
        /// <typeparam name="TProduct">The type of the product.</typeparam>
        /// <param name="exportFactoryContract">The export factory contract.</param>
        /// <param name="definitionAccessor">The definition accessor.</param>
        /// <returns>
        /// The export factory descriptors.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
        public static ExportDescriptorPromise[] GetExportFactoryDescriptors<TProduct>(CompositionContract exportFactoryContract, DependencyAccessor definitionAccessor)
        {
            var productContract = exportFactoryContract.ChangeType(typeof(TProduct));
            // ReSharper disable once NotAccessedVariable
            var boundaries = new string[0];

            IEnumerable<string> specifiedBoundaries;
            CompositionContract unwrapped;
            if (exportFactoryContract.TryUnwrapMetadataConstraint(Constants.SharingBoundaryImportMetadataConstraintName, out specifiedBoundaries, out unwrapped))
            {
                productContract = unwrapped.ChangeType(typeof(TProduct));
                // ReSharper disable once RedundantAssignment
                boundaries = (specifiedBoundaries ?? new string[0]).ToArray();
            }

            return definitionAccessor.ResolveDependencies("product", productContract, false)
                .Select(d => new ExportDescriptorPromise(
                    exportFactoryContract,
                    FormattingHelper.Format(typeof(ExportFactoryAdapter<TProduct>)),
                    false,
                    () => new[] { d },
                    _ =>
                    {
                        var dsc = d.Target.GetDescriptor();
                        var da = dsc.Activator;
                        return ExportDescriptor.Create(
                            (c, o) =>
                            {
                                return new ExportFactoryAdapter<TProduct>(() =>
                                    {
                                        // TODO support lifetime contexts
                                        ////var lifetimeContext = new LifetimeContext(c, boundaries);
                                        var lifetimeContext = c;
                                        return Tuple.Create<TProduct, Action>((TProduct)CompositionOperation.Run(lifetimeContext, da), /* lifetimeContext.Dispose */ () => { });
                                    });
                            },
                            dsc.Metadata);
                    }))
                .ToArray();
        }

        /// <summary>
        /// Gets the export descriptors.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <param name="definitionAccessor">The definition accessor.</param>
        /// <returns>An enumeration of export promises.</returns>
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor definitionAccessor)
        {
            if (!contract.ContractType.IsConstructedGenericType || contract.ContractType.GetGenericTypeDefinition() != typeof(IExportFactory<>))
            {
                // ReSharper disable once ArrangeStaticMemberQualifier
                return ExportDescriptorProvider.NoExportDescriptors;
            }

            var gld = GetExportFactoryDefinitionsMethod.MakeGenericMethod(contract.ContractType.GenericTypeArguments[0]);
            var gldm = gld.CreateStaticDelegate<Func<CompositionContract, DependencyAccessor, object>>();
            return (ExportDescriptorPromise[])gldm(contract, definitionAccessor);
        }
    }
}