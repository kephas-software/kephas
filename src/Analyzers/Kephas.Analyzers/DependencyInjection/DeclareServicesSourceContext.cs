// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSourceContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Kephas.Analyzers.DependencyInjection;

public class DeclareServicesSourceContext(((Compilation, ImmutableArray<TypeDeclarationSyntax>), ImmutableArray<FileScopedNamespaceDeclarationSyntax>) output, SourceProductionContext productionContext)
    : SourceContext<TypeDeclarationSyntax>(output, productionContext), IDeclareServicesSourceContext
{
    private IList<TypeDeclarationSyntax>? serviceDeclarationTypes;
    private IList<TypeDeclarationSyntax>? metadataTypes;
    private IList<ClassDeclarationSyntax>? implementationTypes;

    public IList<TypeDeclarationSyntax> ServiceDeclarationTypes =>
        serviceDeclarationTypes ??= Nodes.Where(t => t.IsServiceDeclarationType()).ToList();

    public IList<TypeDeclarationSyntax> MetadataTypes =>
        metadataTypes ??= Nodes.OfType<TypeDeclarationSyntax>().Where(t => t.CanBeMetadataType()).ToList();

    public IList<ClassDeclarationSyntax> ImplementationTypes =>
        implementationTypes ??= Nodes.OfType<ClassDeclarationSyntax>().Where(t => t.CanBeServiceImplementationService()).ToList();
}