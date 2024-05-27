// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInitializationHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kephas.Analyzers.AssemblyInitialization;

internal static class AssemblyInitializationHelper
{
    private static IEnumerable<string> assemblyInitializerInterfaceIdentifiers = new[]
    {
        "IAssemblyInitializer",
        "Kephas.Runtime.IAssemblyInitializer",
    };

    public static bool IsAssemblyInitializerInterface(this TypeDeclarationSyntax type)
    {
        return assemblyInitializerInterfaceIdentifiers.Contains(type.Identifier.Text);
    }

    public static bool IsAssemblyInitializerInterface(this INamedTypeSymbol type)
    {
        return assemblyInitializerInterfaceIdentifiers.Contains(type.Name);
    }

    public static bool IsAssemblyInitializer(this ClassDeclarationSyntax type, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(type.SyntaxTree);
        var classSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(type)!;

        if (type.BaseList == null)
        {
            return false;
        }

        return Enumerable.Any(classSymbol.AllInterfaces, IsAssemblyInitializerInterface) ||
               classSymbol.BaseType?.IsAssemblyInitializerInterface() is true;
    }
}
