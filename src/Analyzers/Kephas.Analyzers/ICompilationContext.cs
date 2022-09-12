namespace Kephas.Analyzers;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ICompilationContext
{
    public IDictionary<string, FileScopedNamespaceDeclarationSyntax> FileScopedNamespaces { get; }
}