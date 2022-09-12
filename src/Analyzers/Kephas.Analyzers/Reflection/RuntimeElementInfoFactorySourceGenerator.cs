// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeElementInfoFactorySourceGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Analyzers.Reflection;

using System.Text;
using Kephas.Analyzers.Application;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class RuntimeElementInfoFactorySourceGenerator : ISourceGenerator
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

        var factoryTypes = syntaxReceiver.FactoryTypes
            .Where(t => ReflectionHelper.IsFactory(t, context))
            .ToList();
        if (factoryTypes is { Count: 0 })
        {
            return;
        }

        var (initializerNamespace, initializerName) = this.GetAssemblyInitializerClassName(context);

        var source = new StringBuilder();
        source.AppendLine($@"#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

using Kephas.Application;
using Kephas.Runtime;

[assembly: Kephas.CodeAnalysis.AssemblyInitializer(typeof({initializerNamespace}.{initializerName}))]

namespace {initializerNamespace}
{{
    public class {initializerName} : IAssemblyInitializer
    {{
        public void Initialize()
        {{
");
        foreach (var initializerType in factoryTypes)
        {
            source.AppendLine($"            RuntimeTypeRegistry.Instance.RegisterFactory(new {initializerType.GetTypeFullName(syntaxReceiver)}());");
        }

        source.AppendLine($@"
        }}
    }}
}}
");

        context.AddSource("ReflectionInitializer.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
    }

    private (string typeNamespace, string typeName) GetAssemblyInitializerClassName(GeneratorExecutionContext context)
    {
        return ("Kephas.Reflection.Generated",
            $"ReflectionInitializer_{context.Compilation.Assembly.Name.Replace(".", "_")}");
    }

    private class SyntaxReceiver : SyntaxReceiverBase
    {
        public IList<ClassDeclarationSyntax> FactoryTypes { get; } = new List<ClassDeclarationSyntax>();

        public override void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            base.OnVisitSyntaxNode(context);

            if (context.Node is ClassDeclarationSyntax type)
            {
                this.FactoryTypes.Add(type);
            }
        }
    }
}