// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISourceContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Kephas.Analyzers;

using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Provides the compilation context.
/// </summary>
public interface ICompilationContext
{
    /// <summary>
    /// Gets the file scoped namespaces per file.
    /// </summary>
    IDictionary<string, FileScopedNamespaceDeclarationSyntax> FileScopedNamespaces { get; }
}

/// <summary>
/// Provides the compilation context for a filtered output of type <typeparamref name="T"/>.
/// </summary>
public interface ISourceContext<out T> : ICompilationContext
    where T : SyntaxNode
{
    /// <summary>
    /// Gets the compilation.
    /// </summary>
    Compilation Compilation { get; }

    /// <summary>
    /// Gets the source production context.
    /// </summary>
    SourceProductionContext SourceProductionContext { get; }

    /// <summary>
    /// Gets the filtered nodes.
    /// </summary>
    IEnumerable<T> Nodes { get; }
}
