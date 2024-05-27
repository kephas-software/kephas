// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kephas.Analyzers
{
    public class SourceContext<T>(((Compilation, ImmutableArray<T>), ImmutableArray<FileScopedNamespaceDeclarationSyntax>) output, SourceProductionContext productionContext) : ISourceContext<T>
        where T : SyntaxNode
    {
        private IDictionary<string, FileScopedNamespaceDeclarationSyntax>? fileScopedNamespaces;

        public IDictionary<string, FileScopedNamespaceDeclarationSyntax> FileScopedNamespaces
        {
            get => fileScopedNamespaces
                ??= output.Item2
                    .OfType<FileScopedNamespaceDeclarationSyntax>()
                    .Where(ns => !string.IsNullOrEmpty(ns.SyntaxTree.FilePath))
                    .ToImmutableDictionary(
                        ns => ns.SyntaxTree.FilePath,
                        ns => ns);
        }

        public Compilation Compilation => output.Item1.Item1;
        public SourceProductionContext SourceProductionContext { get; } = productionContext;
        public IEnumerable<T> Nodes => output.Item1.Item2;
    }
}
