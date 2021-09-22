// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryWithMetadataExportDescriptorProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The export factory with metadata export descriptor provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.ExportProviders
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Kephas.Injection;
    using Kephas.Injection.SystemComposition.ExportProviders.Metadata;
    using Kephas.Injection.SystemComposition.Internals;
    using Kephas.Reflection;

    /// <summary>
    /// The export factory with metadata export descriptor provider.
    /// </summary>
    public class ExportFactoryWithMetadataExportDescriptorProvider : ExportDescriptorProvider, IExportProvider
    {
        /// <summary>
        /// The GetExportFactoryDescriptors method.
        /// </summary>
        private static readonly MethodInfo GetExportFactoryDescriptorsMethod = typeof(ExportFactoryWithMetadataExportDescriptorProvider).GetTypeInfo()
                                                                            .GetDeclaredMethod(nameof(GetExportFactoryDescriptors));

        /// <summary>
        /// The lifetime context factory.
        /// </summary>
        private static readonly Func<LifetimeContext, string[], LifetimeContext> LifetimeContextFactory;

        /// <summary>
        /// Initializes static members of the <see cref="ExportFactoryWithMetadataExportDescriptorProvider"/>
        /// class.
        /// </summary>
        static ExportFactoryWithMetadataExportDescriptorProvider()
        {
            var lifetimeContextCtor = typeof(LifetimeContext).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(LifetimeContext), typeof(string[]) },
                null);

            LifetimeContextFactory = (parent, sharingBoundaries) =>
                (LifetimeContext)lifetimeContextCtor.Invoke(new object[] { parent, sharingBoundaries });
        }

        /// <summary>
        /// Gets the export factory descriptors.
        /// </summary>
        /// <typeparam name="TProduct">The type of the product.</typeparam>
        /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
        /// <param name="exportFactoryContract">The export factory contract.</param>
        /// <param name="definitionAccessor">The definition accessor.</param>
        /// <returns>The export factory descriptors.</returns>
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
        public static ExportDescriptorPromise[] GetExportFactoryDescriptors<TProduct, TMetadata>(CompositionContract exportFactoryContract, DependencyAccessor definitionAccessor)
        {
            var productContract = exportFactoryContract.ChangeType(typeof(TProduct));
            // ReSharper disable once NotAccessedVariable
            var boundaries = new string[0];

            if (exportFactoryContract.TryUnwrapMetadataConstraint(Constants.SharingBoundaryImportMetadataConstraintName, out IEnumerable<string> specifiedBoundaries, out var unwrapped))
            {
                productContract = unwrapped.ChangeType(typeof(TProduct));
                // ReSharper disable once RedundantAssignment
                boundaries = (specifiedBoundaries ?? new string[0]).ToArray();
            }

            var metadataProvider = MetadataViewProvider.GetMetadataViewProvider<TMetadata>();

            return definitionAccessor.ResolveDependencies("product", productContract, false)
                .Select(d => new ExportDescriptorPromise(
                    exportFactoryContract,
                    typeof(ExportFactoryAdapter<TProduct, TMetadata>).Name,
                    false,
                    () => new[] { d },
                    _ =>
                    {
                        var dsc = d.Target.GetDescriptor();
                        return ExportDescriptor.Create(
                            (c, o) =>
                            {
                                return new ExportFactoryAdapter<TProduct, TMetadata>(
                                    () =>
                                    {
                                        // TODO check how to support lifetime contexts
                                        ////var lifetimeContext = LifetimeContextFactory(c, boundaries);
                                        ////return Tuple.Create<TProduct, Action>((TProduct)CompositionOperation.Run(lifetimeContext, dsc.Activator), lifetimeContext.Dispose);
                                        var lifetimeContext = c;
                                        return Tuple.Create<TProduct, Action>((TProduct)CompositionOperation.Run(lifetimeContext, dsc.Activator), /* lifetimeContext.Dispose */ () => { });
                                    },
                                    metadataProvider(dsc.Metadata));
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
            if (!contract.ContractType.GetTypeInfo().IsGenericType ||
                        contract.ContractType.GetGenericTypeDefinition() != typeof(IExportFactory<,>))
            {
                // ReSharper disable once ArrangeStaticMemberQualifier
                return ExportDescriptorProvider.NoExportDescriptors;
            }

            var ga = contract.ContractType.GenericTypeArguments;
            var gld = GetExportFactoryDescriptorsMethod.MakeGenericMethod(ga[0], ga[1]);
            var gldm = gld.CreateStaticDelegate<Func<CompositionContract, DependencyAccessor, object>>();
            return (ExportDescriptorPromise[])gldm(contract, definitionAccessor);
        }
    }
}