// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncrementalGeneratorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kephas.Analyzers;

/// <summary>
/// Base class for incremental generators.
/// </summary>
/// <param name="filter">Optional. The filter for the selected nodes.</param>
/// <typeparam name="T">The syntax node type.</typeparam>
public abstract class IncrementalGeneratorBase<T>(Func<T, bool>? filter = null) : IIncrementalGenerator
    where T : SyntaxNode
{
    /// <summary>
    /// Called to initialize the generator and register generation steps via callbacks
    /// on the <paramref name="context" />
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.CodeAnalysis.IncrementalGeneratorInitializationContext" /> to register callbacks on</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxProvider = context.SyntaxProvider;
        var fileScopedNamespaces = syntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is FileScopedNamespaceDeclarationSyntax,
                transform: static (ctx, _) => (FileScopedNamespaceDeclarationSyntax)ctx.Node)
            .Where(static a => a is not null);

        var nodes = syntaxProvider
            .CreateSyntaxProvider(
                predicate: (s, _) => s is T node && (filter?.Invoke(node) ?? true),
                transform: static (ctx, _) => (T)ctx.Node)
            .Where(static a => a is not null);

        IncrementalValueProvider<((Compilation, ImmutableArray<T>), ImmutableArray<FileScopedNamespaceDeclarationSyntax>)> compilationAndAttributes
            = context.CompilationProvider.Combine(nodes.Collect()).Combine(fileScopedNamespaces.Collect());

        context.RegisterSourceOutput(compilationAndAttributes, (spc, source) => Execute(CreateSourceContext(source, spc)));
    }

    /// <summary>
    /// The code to be executed upon building.
    /// </summary>
    /// <param name="context">The source compilation context.</param>
    protected abstract void Execute(ISourceContext<T> context);

    /// <summary>
    /// Creates the source context.
    /// </summary>
    /// <param name="source">The filtered syntax nodes.</param>
    /// <param name="spc">The source production context.</param>
    /// <returns>The source context.</returns>
    protected virtual ISourceContext<T> CreateSourceContext(
        ((Compilation, ImmutableArray<T>), ImmutableArray<FileScopedNamespaceDeclarationSyntax>) source,
        SourceProductionContext spc) =>
        new SourceContext<T>(source, spc);
}
