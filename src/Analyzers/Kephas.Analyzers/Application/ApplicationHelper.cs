// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Analyzers.Application;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class ApplicationHelper
{
    private static IEnumerable<string> assemblyInitializerInterfaceIdentifiers = new[]
    {
        "IAssemblyInitializer",
        "Kephas.Application.IAssemblyInitializer",
    };

    public static bool IsAssemblyInitializerInterface(TypeDeclarationSyntax type)
    {
        return assemblyInitializerInterfaceIdentifiers.Contains(type.Identifier.Text);
    }

    public static bool IsAssemblyInitializerInterface(INamedTypeSymbol type)
    {
        return assemblyInitializerInterfaceIdentifiers.Contains(type.Name);
    }

    public static bool IsAssemblyInitializer(ClassDeclarationSyntax type, GeneratorExecutionContext context)
    {
        var semanticModel = context.Compilation.GetSemanticModel(type.SyntaxTree);
        var classSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(type)!;

        if (type.BaseList == null)
        {
            return false;
        }

        foreach (var baseInterface in classSymbol.AllInterfaces)
        {
            if (IsAssemblyInitializerInterface(baseInterface))
            {
                return true;
            }
        }

        return false;
    }
}