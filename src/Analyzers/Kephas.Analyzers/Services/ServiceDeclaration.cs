// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDeclaration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Analyzers.Services
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public record ServiceDeclaration(ClassDeclarationSyntax ServiceType, INamedTypeSymbol? ContractType);
}
