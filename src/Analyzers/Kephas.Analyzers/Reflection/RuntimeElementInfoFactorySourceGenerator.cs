// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeElementInfoFactorySourceGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Analyzers.Reflection;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Source generator for assembly initializers registering runtime element info factories.
/// </summary>
[Generator]
public class RuntimeElementInfoFactorySourceGenerator : IncrementalGeneratorBase<ClassDeclarationSyntax>
{
    /// <summary>
    /// The code to be executed upon building.
    /// </summary>
    /// <param name="context">The source compilation context.</param>
    protected override void Execute(ISourceContext<ClassDeclarationSyntax> context)
    {
        var factoryTypes = context.Nodes
            .Where(t => t.IsFactory(context))
            .ToList();
        if (factoryTypes is { Count: 0 })
        {
            return;
        }

        var (initializerNamespace, initializerName) = GetAssemblyInitializerClassName(context);

        var source = new StringBuilder();
        source.AppendLine(
            $$"""
            #nullable enable

            using System;
            using System.Collections.Generic;
            using System.Diagnostics.CodeAnalysis;
            using System.Reflection;
            using System.Runtime.CompilerServices;

            using Kephas.Runtime;
            using Kephas.Runtime.Factories;
            
            [assembly: AssemblyInitializer<{{initializerNamespace}}.{{initializerName}}>]

            namespace {{initializerNamespace}}
            {
                public class {{initializerName}} : IAssemblyInitializer
                {
                    public void Initialize()
                    {
            """);

        foreach (var initializerType in factoryTypes)
        {
            source.AppendLine(
                $"""
                            RuntimeTypeRegistry.Instance.RegisterFactory(new {initializerType.GetTypeFullName(context)}());
                 """);
        }

        source.AppendLine(
            $$"""
                    }
                }
            }
            """);

        context.SourceProductionContext.AddSource("ReflectionInitializer.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
    }

    private static (string typeNamespace, string typeName) GetAssemblyInitializerClassName(ISourceContext<ClassDeclarationSyntax> context)
    {
        return ("Kephas.Reflection.Generated",
            $"ReflectionInitializer_{context.Compilation.Assembly.Name.Replace(".", "_")}");
    }
}
