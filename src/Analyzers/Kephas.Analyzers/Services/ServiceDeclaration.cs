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

    public class ServiceDeclaration
    {
        public ServiceDeclaration(ClassDeclarationSyntax serviceType, INamedTypeSymbol? contractType)
        {
            this.ServiceType = serviceType;
            this.ContractType = contractType;
        }

        public ClassDeclarationSyntax ServiceType { get; }

        public INamedTypeSymbol? ContractType { get; }
    }
}
