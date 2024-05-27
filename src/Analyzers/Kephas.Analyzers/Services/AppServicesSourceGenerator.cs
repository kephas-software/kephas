// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSourceGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Immutable;

namespace Kephas.Analyzers.Services;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Source generator for collecting application service contracts and types and declaring them at assembly level.
/// </summary>
[Generator]
public class AppServicesSourceGenerator : IncrementalGeneratorBase<TypeDeclarationSyntax>
{
    /// <summary>
    /// The code to be executed upon building.
    /// </summary>
    /// <param name="context">The source compilation context.</param>
    protected override void Execute(ISourceContext<TypeDeclarationSyntax> context)
    {
        var appContext = (IAppServicesSourceContext)context;
        var serviceTypeProvider = this.GetServiceTypeProviderClassName(appContext);

        var source = new StringBuilder();
        source.AppendLine(
            $"""
            #nullable enable

            using System;
            using System.Collections.Generic;
            using System.Diagnostics.CodeAnalysis;

            using Kephas.Services;

            [assembly: AppServices(typeof({serviceTypeProvider.typeNamespace}.{serviceTypeProvider.typeName}))]

            """);
        var isProviderGenerated = AnalyzerServiceHelper.AppendAppServicesProviderClass(
            serviceTypeProvider,
            source,
            appContext,
            appContext.ServiceTypes.Select(t => new ServiceDeclaration(t, AnalyzerServiceHelper.TryGetAppServiceContract(t, appContext))).ToList(),
            appContext);
        if (isProviderGenerated)
        {
            context.SourceProductionContext.AddSource("AppServices.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
        }
    }

    private (string typeNamespace, string typeName) GetServiceTypeProviderClassName(IAppServicesSourceContext context)
    {
        return ("Kephas.Services.Generated", $"AppServices_{context.Compilation.Assembly.Name.Replace(".", "_")}");
    }

    protected override ISourceContext<TypeDeclarationSyntax> CreateSourceContext(((Compilation, ImmutableArray<TypeDeclarationSyntax>), ImmutableArray<FileScopedNamespaceDeclarationSyntax>) source, SourceProductionContext spc)
    {
        return new AppServicesSourceContext(source, spc);
    }
}
