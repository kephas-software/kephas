// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSourceGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Kephas.Analyzers.DependencyInjection;

/// <summary>
/// Source generator for collecting service types and implementation types, and declaring them at assembly level.
/// </summary>
[Generator]
public class DeclareServicesSourceGenerator : IncrementalGeneratorBase<TypeDeclarationSyntax>
{
    /// <summary>
    /// The code to be executed upon building.
    /// </summary>
    /// <param name="context">The source compilation context.</param>
    protected override void Execute(ISourceContext<TypeDeclarationSyntax> context)
    {
        var sourceContext = (IDeclareServicesSourceContext)context;
        var serviceTypeProvider = GetServiceTypeProviderClassName(sourceContext);

        var source = new StringBuilder();
        source.AppendLine(
            $$"""
            #nullable enable

            using System;
            using System.Collections.Generic;
            using System.Diagnostics.CodeAnalysis;
            using System.Reflection;

            using Kephas.DependencyInjection;
            using Kephas.DependencyInjection.ServiceProviders;

            [assembly: DeclareServices<{{serviceTypeProvider.typeNamespace}}.{{serviceTypeProvider.typeName}}>()]
            
            """);
        var isProviderGenerated = DependencyInjectionAnalyzerHelper.AppendServicesInfoProviderClass(
            serviceTypeProvider,
            source,
            sourceContext,
            sourceContext.ImplementationTypes.Select(t => new ServiceImplementationDeclaration(t, DependencyInjectionAnalyzerHelper.TryGetServiceDeclarationType(t, sourceContext))).ToList());
        if (isProviderGenerated)
        {
            context.SourceProductionContext.AddSource("DeclaredServices.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
        }
    }

    private static (string typeNamespace, string typeName) GetServiceTypeProviderClassName(IDeclareServicesSourceContext context)
    {
        return ("Kephas.DependencyInjection.ServiceProviders.Generated", $"DeclareServices_{context.Compilation.Assembly.Name.Replace(".", "_")}");
    }

    /// <summary>
    /// Creates the source context.
    /// </summary>
    /// <param name="source">The filtered syntax nodes.</param>
    /// <param name="spc">The source production context.</param>
    /// <returns>The source context.</returns>
    protected override ISourceContext<TypeDeclarationSyntax> CreateSourceContext(((Compilation, ImmutableArray<TypeDeclarationSyntax>), ImmutableArray<FileScopedNamespaceDeclarationSyntax>) source, SourceProductionContext spc)
    {
        return new DeclareServicesSourceContext(source, spc);
    }
}
