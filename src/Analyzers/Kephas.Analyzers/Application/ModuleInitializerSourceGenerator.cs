// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializerSourceGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Analyzers.Application;

using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class ModuleInitializerSourceGenerator : ISourceGenerator
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

        var (initializerNamespace, initializerName) = this.GetModuleInitializerClassName(context);

        var source = new StringBuilder();
        source.AppendLine($@"#nullable enable

#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

using Kephas.Application;
using Kephas.Runtime;

namespace {initializerNamespace}
{{
    internal static class {initializerName}
    {{
        [ModuleInitializer]
        internal static void InitializeModule()
        {{
            IAssemblyInitializer initializer;");

        foreach (var initializerType in syntaxReceiver.InitializerTypes
                     .Where(t => ApplicationHelper.IsAssemblyInitializer(t, context)))
        {
            source.AppendLine($"            initializer = (IAssemblyInitializer)new {initializerType.GetTypeFullName(syntaxReceiver)}();");
            source.AppendLine($"            initializer.Initialize();");
        }

        source.AppendLine($@"
            var thisAssembly = Assembly.GetExecutingAssembly();
            foreach (var attr in thisAssembly.GetCustomAttributes<AssemblyInitializerAttribute>())
            {{
                foreach (var initializerType in attr.InitializerTypes)
                {{
                    try 
                    {{
                        if (initializerType.IsClass && !initializerType.IsAbstract && Activator.CreateInstance(initializerType) is IAssemblyInitializer runtimeInitializer)
                        {{
                            runtimeInitializer.Initialize();
                        }}
                    }}
                    catch (Exception ex)
                    {{
                        // write exception somewhere...
                    }}
                }}
            }}
        }}
    }}
}}

#endif
");

        context.AddSource("ModuleInitializer.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
    }

    private (string typeNamespace, string typeName) GetModuleInitializerClassName(GeneratorExecutionContext context)
    {
        return ("Kephas.Application.Generated",
            $"ModuleInitializer_{context.Compilation.Assembly.Name.Replace(".", "_")}");
    }

    private class SyntaxReceiver : SyntaxReceiverBase
    {
        public IList<ClassDeclarationSyntax> InitializerTypes { get; } = new List<ClassDeclarationSyntax>();

        public override void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            base.OnVisitSyntaxNode(context);

            if (context.Node is ClassDeclarationSyntax type)
            {
                this.InitializerTypes.Add(type);
            }
        }
    }
}
