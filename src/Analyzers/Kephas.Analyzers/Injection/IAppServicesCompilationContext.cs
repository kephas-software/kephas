// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServicesCompilationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kephas.Analyzers.Injection;

public interface IAppServicesCompilationContext
{
    public IList<TypeDeclarationSyntax> ContractTypes { get; }

    public IList<ClassDeclarationSyntax> ServiceTypes { get; }

    public IDictionary<string, FileScopedNamespaceDeclarationSyntax> FileScopedNamespaces { get; }
}