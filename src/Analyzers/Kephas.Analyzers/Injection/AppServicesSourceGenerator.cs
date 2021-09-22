﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSourceGenerator.cs" company="Kephas Software SRL">
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
    /// Source generator for collecting application service contracts and types and declaring them at assembly level.
    /// </summary>
    [Generator]
    public class AppServicesSourceGenerator : ISourceGenerator
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

            var serviceTypeProvider = this.GetServiceTypeProviderClassName(context);

            var source = new StringBuilder();
            source.AppendLine($@"#nullable enable

using System;
using System.Collections.Generic;

using Kephas.Services;

[assembly: AppServices(typeof({serviceTypeProvider.typeNamespace}.{serviceTypeProvider.typeName}))]

");

            var isProviderGenerated = this.AppendAppServicesProviderClass(serviceTypeProvider, source, context, syntaxReceiver.ContractTypes, syntaxReceiver.ServiceTypes);
            if (isProviderGenerated)
            {
                context.AddSource("AppServices.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
            }
        }

        private (string typeNamespace, string typeName) GetServiceTypeProviderClassName(GeneratorExecutionContext context)
        {
            return ("Kephas.Injection.Generated", $"AppServices_{context.Compilation.Assembly.Name.Replace(".", "_")}");
        }

        private bool AppendAppServicesProviderClass(
            (string typeNamespace, string typeName) serviceTypeProvider,
            StringBuilder source,
            GeneratorExecutionContext context,
            IList<TypeDeclarationSyntax> contractTypes,
            IList<ClassDeclarationSyntax> serviceTypes)
        {
            var isProviderEmpty = true;

            source.AppendLine($@"namespace {serviceTypeProvider.typeNamespace}");
            source.AppendLine($@"{{");
            source.AppendLine($@"   public class {serviceTypeProvider.typeName}: IAppServiceTypesProvider, IAppServiceInfosProvider");
            source.AppendLine($@"   {{");
            source.AppendLine($@"       public IEnumerable<Type>? GetContractDeclarationTypes(dynamic? context = null)");
            source.AppendLine($@"       {{");

            if (contractTypes.Count > 0)
            {
                var contractTypesBuilder = new StringBuilder();
                foreach (var typeSyntax in contractTypes)
                {
                    var typeFullName = InjectionHelper.GetTypeFullName(typeSyntax);
                    source.AppendLine($"            yield return typeof({typeFullName});");
                    contractTypesBuilder.Append($"{typeSyntax.Identifier}, ");
                }

                isProviderEmpty = false;

                contractTypesBuilder.Length -= 2;

                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("KG1000", nameof(AppServicesSourceGenerator), $"Identified following application service contracts: {contractTypesBuilder}.", "Kephas", DiagnosticSeverity.Info, isEnabledByDefault: true), Location.None));
            }
            else
            {
                source.AppendLine($"            yield break;");
            }

            source.AppendLine($@"       }}");
            source.AppendLine();
            source.AppendLine($@"       public IEnumerable<(Type serviceType, Type contractDeclarationType)> GetAppServiceTypes(dynamic? context = null)");
            source.AppendLine($@"       {{");

            if (serviceTypes.Count > 0)
            {
                var serviceTypesBuilder = new StringBuilder();
                foreach (var classSyntax in serviceTypes)
                {
                    var appServiceContract = InjectionHelper.TryGetAppServiceContract(classSyntax, context);
                    if (appServiceContract == null)
                    {
                        continue;
                    }

                    var typeFullName = InjectionHelper.GetTypeFullName(classSyntax);
                    source.AppendLine($"            yield return (typeof({typeFullName}), typeof({InjectionHelper.GetTypeFullName(appServiceContract)}));");
                    serviceTypesBuilder.Append($"{classSyntax.Identifier}, ");

                    isProviderEmpty = false;
                }

                if (serviceTypesBuilder.Length > 0)
                {
                    serviceTypesBuilder.Length -= 2;

                    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("KG1000", nameof(AppServicesSourceGenerator), $"Identified following application service contracts: {serviceTypesBuilder}.", "Kephas", DiagnosticSeverity.Info, isEnabledByDefault: true), Location.None));
                }
                else
                {
                    source.AppendLine($"            yield break;");
                }
            }

            source.AppendLine($@"       }}");
            source.AppendLine($@"   }}");
            source.AppendLine($@"}}");

            return !isProviderEmpty;
        }

        private void AppendServiceProviderTypes((string typeNamespace, string typeName) serviceTypeProvider, StringBuilder source)
        {
            source.Append($@"   serviceProviderTypes: new Type[] {{ typeof({serviceTypeProvider.typeNamespace}.{serviceTypeProvider.typeName}) }}");
        }

        private void AppendContractDeclarationTypes(StringBuilder source, GeneratorExecutionContext context, IList<TypeDeclarationSyntax> contractTypes)
        {
            source.AppendLine($@"   contractDeclarationTypes: new Type[] {{");

            var contractTypesBuilder = new StringBuilder();
            if (contractTypes.Count > 0)
            {
                foreach (var typeSyntax in contractTypes)
                {
                    var typeFullName = InjectionHelper.GetTypeFullName(typeSyntax);
                    source.AppendLine($"        typeof({typeFullName}),");
                    contractTypesBuilder.Append($"{typeSyntax.Identifier}, ");
                }

                contractTypesBuilder.Length -= 2;
                source.Length -= Environment.NewLine.Length;
                source.Length -= 1;
                source.AppendLine();

                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("KG1000", nameof(AppServicesSourceGenerator), $"Identified following application service contracts: {contractTypesBuilder}.", "Kephas", DiagnosticSeverity.Info, isEnabledByDefault: true), Location.None));
            }

            source.Append($@"   }}");
        }

        private class SyntaxReceiver : ISyntaxContextReceiver
        {
            internal IList<TypeDeclarationSyntax> ContractTypes = new List<TypeDeclarationSyntax>();
            internal IList<ClassDeclarationSyntax> ServiceTypes = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                // find all classes and interfaces marked with [AppServiceContract] attributes.
                if (context.Node is TypeDeclarationSyntax contract
                    && InjectionHelper.IsAppServiceContract(contract))
                {
                    this.ContractTypes.Add(contract);
                }

                // find all classes which are potentially application services.
                if (context.Node is ClassDeclarationSyntax type
                    && InjectionHelper.CanBeAppService(type))
                {
                    this.ServiceTypes.Add(type);
                }
            }
        }
    }
}