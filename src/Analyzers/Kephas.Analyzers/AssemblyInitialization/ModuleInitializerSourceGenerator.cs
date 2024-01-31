// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializerSourceGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Kephas.Analyzers.AssemblyInitialization;

/*
 * Documentation for creating source code generators can be found here:
 * https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/
 */

/// <summary>
/// Source generator for module initializers.
/// </summary>
[Generator]
public class ModuleInitializerSourceGenerator : IncrementalGeneratorBase<ClassDeclarationSyntax>
{
    /// <summary>
    /// The code to be executed upon building.
    /// </summary>
    /// <param name="context">The source compilation context.</param>
    protected override void Execute(ISourceContext<ClassDeclarationSyntax> context)
    {
        var compilation = context.Compilation;
        var (initializerNamespace, initializerName) = this.GetModuleInitializerClassName(compilation);

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

            namespace {{initializerNamespace}}
            {
                internal static class {{initializerName}}
                {
                    [ModuleInitializer]
                    internal static void InitializeModule()
                    {
                        IAssemblyInitializer initializer;
            """);

        foreach (var initializerType in context.Nodes
                     .Where(t => t.IsAssemblyInitializer(compilation)))
        {
            source.AppendLine(
            $"""
                         initializer = (IAssemblyInitializer)new {initializerType.GetTypeFullName(context)}();
             """);
            source.AppendLine(
            $"""
                         initializer.InitializeAssembly();
             """);
        }

        source.AppendLine(
            $$"""
                        var thisAssembly = Assembly.GetExecutingAssembly();
                        foreach (var assemblyInitializer in thisAssembly.GetCustomAttributes<Attribute>().OfType<IAssemblyInitializer>())
                        {
                            try 
                            {
                                assemblyInitializer.InitializeAssembly();
                            }
                            catch (Exception ex)
                            {
                                // write exception somewhere...
                            }
                        }
                    }
                }
            }
            """);

        context.SourceProductionContext.AddSource("ModuleInitializer.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
    }

    private (string typeNamespace, string typeName) GetModuleInitializerClassName(Compilation compilation)
    {
        return ("Kephas.AssemblyInitialization.Generated",
            $"ModuleInitializer_{compilation.Assembly.Name.Replace(".", "_")}");
    }
}
