// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractSourceGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Analyzers.Injection
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Source generator for collecting application service contracts and declaring them at assembly level.
    /// </summary>
    [Generator]
    public class AppServiceContractSourceGenerator : ISourceGenerator
    {
        /// <summary>
        /// Called before generation occurs. A generator can use the <paramref name="context" />
        /// to register callbacks required to perform generation.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.CodeAnalysis.GeneratorInitializationContext" /> to register callbacks on.</param>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/Microsoft.CodeAnalysis.ISourceGenerator.Initialize?view=netstandard-2.0">`ISourceGenerator.Initialize` on docs.microsoft.com</a></footer>
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        /// <summary>
        /// Called to perform source generation. A generator can use the <paramref name="context" />
        /// to add source files via the <see cref="M:Microsoft.CodeAnalysis.GeneratorExecutionContext.AddSource(System.String,Microsoft.CodeAnalysis.Text.SourceText)" />
        /// method.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.CodeAnalysis.GeneratorExecutionContext" /> to add source to.</param>
        /// <remarks>
        /// This call represents the main generation step. It is called after a <see cref="T:Microsoft.CodeAnalysis.Compilation" /> is
        /// created that contains the user written code.
        /// A generator can use the <see cref="P:Microsoft.CodeAnalysis.GeneratorExecutionContext.Compilation" /> property to
        /// discover information about the users compilation and make decisions on what source to
        /// provide.
        /// </remarks>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/Microsoft.CodeAnalysis.ISourceGenerator.Execute?view=netstandard-2.0">`ISourceGenerator.Execute` on docs.microsoft.com</a></footer>
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReceiver = (SyntaxReceiver)context.SyntaxContextReceiver!;
            var contractTypes = syntaxReceiver.ContractTypes;
            if (contractTypes.Count == 0)
            {
                return;
            }

            var types = new StringBuilder();
            var source = new StringBuilder();
            source.Append($@"
using Kephas.Services;

[assembly: AppServiceInfoProvider(");
            foreach (var typeSyntax in contractTypes)
            {
                var typeFullName = InjectionHelper.GetTypeFullName(typeSyntax);
                source.AppendLine($"typeof({typeFullName}), ");
                types.Append($"{typeSyntax.Identifier}, ");
            }

            types.Length -= 2;
            source.Length -= Environment.NewLine.Length;
            source.Length -= 2;
            source.Append($@")]
");
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("KG1000", typeof(AppServiceContractSourceGenerator).Name, $"Identified following application service contracts: {types}.", "Kephas", DiagnosticSeverity.Info, isEnabledByDefault: true), Location.None));
            context.AddSource("Kephas.AppServiceContracts.cs", SourceText.From(source.ToString(), Encoding.UTF8));
        }

        private class SyntaxReceiver : ISyntaxContextReceiver
        {
            internal IList<TypeDeclarationSyntax> ContractTypes = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                // find all classes and interfaces marked with [AppServiceContract] attributes.
                if (context.Node is TypeDeclarationSyntax typeNode
                    && InjectionHelper.IsAppServiceContract(typeNode))
                {
                    this.ContractTypes.Add(typeNode);
                }
            }
        }
    }
}