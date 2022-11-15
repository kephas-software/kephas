namespace Kephas.Analyzers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class SyntaxReceiverBase : ISyntaxContextReceiver, ICompilationContext
{
    public IDictionary<string, FileScopedNamespaceDeclarationSyntax> FileScopedNamespaces { get; } =
        new Dictionary<string, FileScopedNamespaceDeclarationSyntax>();

    public virtual void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        // store all file scoped namespaces
        if (context.Node is FileScopedNamespaceDeclarationSyntax fileScopedNamespace)
        {
            if (!string.IsNullOrEmpty(context.Node.SyntaxTree.FilePath))
            {
                this.FileScopedNamespaces[context.Node.SyntaxTree.FilePath] = fileScopedNamespace;
            }
        }
    }
}