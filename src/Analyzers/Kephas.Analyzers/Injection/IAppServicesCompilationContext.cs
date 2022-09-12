// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServicesCompilationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Analyzers.Injection;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface IAppServicesCompilationContext : ICompilationContext
{
    public IList<TypeDeclarationSyntax> ContractTypes { get; }

    public IList<ClassDeclarationSyntax> MetadataTypes { get; }

    public IList<ClassDeclarationSyntax> ServiceTypes { get; }
}