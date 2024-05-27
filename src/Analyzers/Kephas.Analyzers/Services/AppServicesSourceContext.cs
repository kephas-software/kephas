// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSourceContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kephas.Analyzers.Services
{
    public class AppServicesSourceContext(((Compilation, ImmutableArray<TypeDeclarationSyntax>), ImmutableArray<FileScopedNamespaceDeclarationSyntax>) output, SourceProductionContext productionContext)
        : SourceContext<TypeDeclarationSyntax>(output, productionContext), IAppServicesSourceContext
    {
        private IList<TypeDeclarationSyntax>? contractTypes;
        private IList<ClassDeclarationSyntax>? metadataTypes;
        private IList<ClassDeclarationSyntax>? serviceTypes;

        public IList<TypeDeclarationSyntax> ContractTypes =>
            contractTypes ??= Nodes.Where(t => t.IsAppServiceContract()).ToList();

        public IList<ClassDeclarationSyntax> MetadataTypes =>
            metadataTypes ??= Nodes.OfType<ClassDeclarationSyntax>().Where(t => t.CanBeMetadataType()).ToList();

        public IList<ClassDeclarationSyntax> ServiceTypes =>
            serviceTypes ??= Nodes.OfType<ClassDeclarationSyntax>().Where(t => t.CanBeAppService()).ToList();
    }
}
