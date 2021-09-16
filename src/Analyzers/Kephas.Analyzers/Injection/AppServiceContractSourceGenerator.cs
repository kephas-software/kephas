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
    using System.Linq;
    using System.Text;

    using Kephas.Injection.AttributedModel;
    using Kephas.Services;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Source generator for application service contracts.
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

            var sb = new StringBuilder();
            sb.Append($@"
using Kephas.Services;

[assembly: AppServiceInfoProvider(");
            foreach (var typeSyntax in contractTypes)
            {
                sb.AppendLine($"typeof({this.GetTypeFullName(typeSyntax)}), ");
            }

            sb.Length -= 2;
            sb.Append($@")]
");
            context.AddSource("app-service-contracts", sb.ToString());
        }

        private string GetTypeFullName(TypeDeclarationSyntax typeSyntax)
        {
            var sb = new StringBuilder();

            var parent = typeSyntax.Parent;
            while (parent != null)
            {
                switch (parent)
                {
                    case TypeDeclarationSyntax parentTypeDeclSyntax:
                        sb.Insert(0, '.');
                        sb.Insert(0, this.GetTypeFullName(parentTypeDeclSyntax));
                        parent = null;
                        break;
                    case NamespaceDeclarationSyntax namespaceSyntax:
                        sb.Insert(0, '.');
                        sb.Insert(0, namespaceSyntax.Name.ToString());
                        break;
                }

                parent = parent?.Parent;
            }

            if (sb.Length > 0)
            {
                sb.Append('.');
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

        private class SyntaxReceiver : ISyntaxContextReceiver
        {
            internal IList<TypeDeclarationSyntax> ContractTypes = new List<TypeDeclarationSyntax>();

            private const string AttributeEnding = "Attribute";
            private static readonly List<string> ExcludedAttrs;
            private static readonly List<string> AppServiceContractAttrs;

            static SyntaxReceiver()
            {
                ExcludedAttrs = GetAttrNames(typeof(ExcludeFromInjectionAttribute)).ToList();
                AppServiceContractAttrs = new List<string>();
                AppServiceContractAttrs.AddRange(GetAttrNames(typeof(AppServiceContractAttribute)));
                AppServiceContractAttrs.AddRange(GetAttrNames(typeof(SingletonAppServiceContractAttribute)));
                AppServiceContractAttrs.AddRange(GetAttrNames(typeof(ScopedAppServiceContractAttribute)));
            }

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                // find all classes and interfaces marked with [AppServiceContract] attributes.
                if (context.Node is TypeDeclarationSyntax typeNode
                    && this.IsAppServiceContract(typeNode.AttributeLists))
                {
                    this.ContractTypes.Add(typeNode);
                }
            }

            private static IEnumerable<string> GetAttrNames(Type attributeType)
            {
                yield return attributeType.Name;
                yield return attributeType.FullName;
                yield return attributeType.Name.Substring(0, attributeType.Name.Length - AttributeEnding.Length);
                yield return attributeType.FullName!.Substring(0, attributeType.FullName.Length - AttributeEnding.Length);
            }

            private bool IsAppServiceContract(SyntaxList<AttributeListSyntax> attrLists)
            {
                var attrs = attrLists.SelectMany(al => al.Attributes).ToList();
                return !attrs.Any(a => this.ContainsAttribute(a, ExcludedAttrs))
                       && attrs.Any(a => this.ContainsAttribute(a, AppServiceContractAttrs));
            }

            private bool ContainsAttribute(AttributeSyntax attributeSyntax, IEnumerable<string> attrs)
            {
                var attrName = attributeSyntax.Name.ToString();
                return attrs.Contains(attrName);
            }
        }
    }
}