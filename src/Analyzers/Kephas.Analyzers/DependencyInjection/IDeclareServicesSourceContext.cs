// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServicesSourceContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Kephas.Analyzers.DependencyInjection;

public interface IDeclareServicesSourceContext : ISourceContext<TypeDeclarationSyntax>
{
    public IList<TypeDeclarationSyntax> ServiceDeclarationTypes { get; }

    public IList<ClassDeclarationSyntax> MetadataTypes { get; }

    public IList<ClassDeclarationSyntax> ImplementationTypes { get; }
}
