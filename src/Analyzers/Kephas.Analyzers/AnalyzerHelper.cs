// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalyzerHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Analyzers;

using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class AnalyzerHelper
{
    public static string GetTypeFullName(this TypeDeclarationSyntax typeSyntax, ICompilationContext compilationContext)
    {
        var sb = new StringBuilder();

        var parent = typeSyntax.Parent;
        while (parent != null)
        {
            switch (parent)
            {
                case TypeDeclarationSyntax parentTypeDeclSyntax:
                    sb.Insert(0, '.');
                    sb.Insert(0, GetTypeFullName(parentTypeDeclSyntax, compilationContext));
                    parent = null;
                    break;
                case NamespaceDeclarationSyntax namespaceSyntax:
                    sb.Insert(0, '.');
                    sb.Insert(0, namespaceSyntax.Name.ToString());
                    break;
            }

            parent = parent?.Parent;
        }

        var fileScopedNamespace = GetFileScopedNamespace(typeSyntax, compilationContext);
        if (fileScopedNamespace != null)
        {
            sb.Insert(0, '.');
            sb.Insert(0, fileScopedNamespace);
        }

        sb.Append(typeSyntax.Identifier.Text);

        if (typeSyntax.TypeParameterList != null)
        {
            sb.Append('<');
            for (var i = 0; i < typeSyntax.TypeParameterList.Parameters.Count - 1; i++)
            {
                sb.Append(',');
            }

            sb.Append('>');
        }

        return sb.ToString();
    }

    private static string? GetFileScopedNamespace(TypeDeclarationSyntax typeSyntax,
        ICompilationContext compilationContext)
    {
        if (!string.IsNullOrEmpty(typeSyntax.SyntaxTree.FilePath) &&
            compilationContext.FileScopedNamespaces.TryGetValue(typeSyntax.SyntaxTree.FilePath,
                out var fileScopedNamespace))
        {
            return fileScopedNamespace.Name.ToString();
        }

        return null;
    }
}