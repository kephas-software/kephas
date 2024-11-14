// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextSourceGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Kephas.Analyzers.EntityFrameworkCore;

/// <summary>
/// Source generator for generating entity context types.
/// </summary>
[Generator]
public class EntityContextSourceGenerator()
    : IncrementalGeneratorBase<AttributeSyntax>(a => a.Name.ToString().StartsWith(GenerateEntityContextAttributeName))
{
    private const string GenerateEntityContextAttributeName = "GenerateEntityContext";
    private const string SpArgsEnding = "SpArgs";

    /// <summary>
    /// The code to be executed upon building.
    /// </summary>
    /// <param name="context">The source compilation context.</param>
    protected override void Execute(ISourceContext<AttributeSyntax> context)
    {
        foreach (var attribute in context.Nodes)
        {
            var arguments = attribute.ArgumentList?.Arguments!;
            if (!TryGetEntityContextName(arguments, out string entityContextName))
            {
                continue;
            }

            var serviceTypeProvider = GetServiceTypeProviderClassName(context, entityContextName);
            var entityContextTypeName = $"{entityContextName}EntityContext";

            string priority = GetPriority(arguments.Value);
            var connectionName = GetConnectionName(arguments.Value, entityContextName);
            var entityTypes = arguments!.Value
                .Select(a => a.Expression as TypeOfExpressionSyntax)
                .Where(t => t is not null)
                .ToList();

            // first add the usings / namespaces used by models, so that the DatContext class
            // can use the entity types without errors
            var source = new StringBuilder();
            source.AppendLine(
                $$"""
                  #nullable enable
                  
                  // Generated for {{context.Compilation.Assembly}}

                  {{GetUsingsForEntityTypes(attribute)}}
                  
                  [assembly: DeclareServices<{{serviceTypeProvider.typeNamespace}}.{{serviceTypeProvider.typeName}}>()]
                  
                  namespace {{serviceTypeProvider.typeNamespace}};

                  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
                  internal partial class {{serviceTypeProvider.typeName}}: IServiceInfoProvider
                  {
                      IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
                      {
                           yield break;
                      }
                  
                      IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
                      {
                           yield return new ServiceImplementationDeclaration(typeof({{entityContextTypeName}}), typeof(IEntityContext<>));
                      }
                  }
                  
                  [Priority({{priority}})]
                  [ConnectionName({{connectionName}})]
                  [EntityTypes(
                  {{GetEntityTypeList("    ", entityTypes!)}}
                  )]
                  [GeneratedCode("{{nameof(EntityContextSourceGenerator)}}", "{{Assembly.GetExecutingAssembly().GetName().Version}}")]
                  public partial class {{entityContextTypeName}} : EntityContextBase<{{entityContextTypeName}}>
                  {
                      public {{entityContextTypeName}}(DbContextOptions<{{entityContextTypeName}}> options, IDbContextBuilder builder)
                          : base(options, builder)
                      {
                      }
                  """);

            foreach (var entityType in entityTypes)
            {
                var (entityTypeName, entityTypeFullName) = GetEntityTypeInfo(entityType!);
                if (ShouldExcludeEntityType(entityTypeName))
                {
                    continue;
                }

                // for each model type add a property
                source.AppendLine(
                    $$"""
                        public DbSet<{{entityTypeFullName}}> {{entityTypeName}} { get; set; } = null!;
                    """);

            }

            source.AppendLine(
                $$"""
                  }
                  """);

            context.SourceProductionContext.AddSource($"{entityContextName}EntityContext.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
        }
    }

    private static (string typeNamespace, string typeName) GetServiceTypeProviderClassName(ISourceContext<AttributeSyntax> context, string entityContextName)
    {
        return ($"{context.Compilation.Assembly.Name}.EntityContexts.Generated", $"DeclareServices_{entityContextName}_{context.Compilation.Assembly.Name.Replace(".", "_")}");
    }

    private static string GetPriority(SeparatedSyntaxList<AttributeArgumentSyntax> arguments)
    {
        var priority = arguments
                           .FirstOrDefault(a => a.ToString().StartsWith("Priority."))?
                           .ToString()
                       ?? "Priority.Normal";
        return priority;
    }

    private static string GetConnectionName(SeparatedSyntaxList<AttributeArgumentSyntax> arguments, string entityContextName)
    {
        if (arguments.Count > 1)
        {
            var connectionNameArg = arguments[1];
            if (connectionNameArg.Expression is TypeOfExpressionSyntax || connectionNameArg.ToString().StartsWith("Priority."))
            {
                return $"\"{entityContextName}\"";
            }
            
            return connectionNameArg.ToString();
        }

        return $"\"{entityContextName}\"";
    }

    private static bool TryGetEntityContextName(SeparatedSyntaxList<AttributeArgumentSyntax>? arguments, out string entityContextName)
    {
        entityContextName = arguments?[0].ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(entityContextName))
        {
            return false;
        }

        // trim the quotation marks
        if (entityContextName!.StartsWith("\""))
        {
            entityContextName = entityContextName.Substring(1, entityContextName.Length - 2);
        }

        return true;
    }

    private static string GetEntityTypeList(string indent, IEnumerable<TypeOfExpressionSyntax> entityTypes)
    {
        using var enumerator = entityTypes.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        sb.Append(indent).Append(enumerator.Current!.ToString());
        while (enumerator.MoveNext())
        {
            sb.AppendLine().Append(indent).Append(", ").Append(enumerator.Current!);
        }
        return sb.ToString();
    }

    private static bool ShouldExcludeEntityType(string? entityTypeName)
    {
        return entityTypeName switch
        {
            null => true,
            _ when entityTypeName.EndsWith(SpArgsEnding) => true,
            _ => false,
        };
    }

    private static (string? entityTypeName, string? entityTypeFullName) GetEntityTypeInfo(TypeOfExpressionSyntax entityType)
    {
        var typeFullName = entityType.Type.ToString();
        var indexOfDot = typeFullName.IndexOf('.');
        var typeName = indexOfDot <= 0 ? typeFullName : typeFullName.Substring(indexOfDot + 1);
        return (typeName, typeFullName);
    }

    private static string GetUsingsForEntityTypes(AttributeSyntax attributeSyntax)
    {
        var parent = attributeSyntax.Parent;

        while (parent is not null and not CompilationUnitSyntax)
            parent = parent.Parent;

        if (parent is null) return string.Empty;

        var compilationUnit = (CompilationUnitSyntax)parent;

        var usings = new List<string>
        {
            "using System;",
            "using System.Collections.Generic;",
            "using System.Diagnostics.CodeAnalysis;",
            "using System.Reflection;",
            "using System.CodeDom.Compiler;",

            "using Microsoft.EntityFrameworkCore;",
            "using Kephas.DependencyInjection;",
            "using Kephas.DependencyInjection.Metadata;",
            "using Kephas.DependencyInjection.ServiceProviders;",
            "using Kephas.Entities;",
            "using Kephas.Entities.Builder;",
        };

        usings.AddRange(
            compilationUnit.Usings.Select(u => u.ToFullString()
                .TrimEnd('\r', '\n')));
        return string.Join("\n", usings.Distinct());
    }
}