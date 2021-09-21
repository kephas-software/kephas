// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionHelper.cs" company="Kephas Software SRL">
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

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper class for injection source generators.
    /// </summary>
    public static class InjectionHelper
    {
        private const string AttributeEnding = "Attribute";
        private static readonly List<string> ExcludedAttrs;
        private static readonly List<string> AppServiceContractAttrs;

        static InjectionHelper()
        {
            ExcludedAttrs = GetAttrNames("Kephas.Injection.AttributedModel.ExcludeFromInjectionAttribute").ToList();
            AppServiceContractAttrs = new List<string>();
            AppServiceContractAttrs.AddRange(GetAttrNames("Kephas.Services.AppServiceContractAttribute"));
            AppServiceContractAttrs.AddRange(GetAttrNames("Kephas.Services.SingletonAppServiceContractAttribute"));
            AppServiceContractAttrs.AddRange(GetAttrNames("Kephas.Services.ScopedAppServiceContractAttribute"));
        }

        public static bool IsAppServiceContract(TypeDeclarationSyntax type)
        {
            var attrLists = type.AttributeLists;
            var attrs = attrLists.SelectMany(al => al.Attributes).ToList();
            return !attrs.Any(a => ContainsAttribute(a, ExcludedAttrs))
                   && attrs.Any(a => ContainsAttribute(a, AppServiceContractAttrs))
                   && type.Modifiers.Any(m => m.ValueText == "public")
                   && type.Modifiers.All(m => m.ValueText != "static");
        }

        public static bool IsAppServiceContract(INamedTypeSymbol type)
        {
            var attrs = type.GetAttributes();
            return !attrs.Any(a => ContainsAttribute(a, ExcludedAttrs))
                   && attrs.Any(a => ContainsAttribute(a, AppServiceContractAttrs))
                   && !type.IsStatic
                   && type.DeclaredAccessibility == Accessibility.Public;
        }

        public static INamedTypeSymbol? TryGetAppServiceContract(ClassDeclarationSyntax type, GeneratorExecutionContext context)
        {
            var semanticModel = context.Compilation.GetSemanticModel(type.SyntaxTree);
            var classSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(type)!;

            if (IsAppServiceContract(type))
            {
                return classSymbol;
            }

            if (type.BaseList == null)
            {
                return null;
            }

            foreach (var baseInterface in classSymbol.AllInterfaces)
            {
                if (IsAppServiceContract(baseInterface))
                {
                    return baseInterface;
                }
            }

            if (classSymbol.BaseType != null && IsAppServiceContract(classSymbol.BaseType))
            {
                return classSymbol.BaseType;
            }

            return null;
        }

        public static bool CanBeAppService(ClassDeclarationSyntax type)
        {
            var attrLists = type.AttributeLists;
            var attrs = attrLists.SelectMany(al => al.Attributes).ToList();
            if (attrs.Any(a => ContainsAttribute(a, ExcludedAttrs)))
            {
                return false;
            }

            return type.Modifiers.Any(m => m.ValueText == "public")
                && type.Modifiers.All(m => m.ValueText != "static")
                && type.Modifiers.All(m => m.ValueText != "abstract");
        }

        public static string GetTypeFullName(INamedTypeSymbol typeSymbol)
        {
            var fullName = typeSymbol.Name;
            var ns = typeSymbol.ContainingNamespace;
            while (ns != null)
            {
                if (!string.IsNullOrEmpty(ns.Name))
                {
                    fullName = ns.Name + "." + fullName;
                }

                ns = ns.ContainingNamespace;
            }

            return fullName;
        }

        public static string GetTypeFullName(TypeDeclarationSyntax typeSyntax)
        {
            var sb = new StringBuilder();

            var parent = typeSyntax.Parent;
            while (parent != null)
            {
                switch (parent)
                {
                    case TypeDeclarationSyntax parentTypeDeclSyntax:
                        sb.Insert(0, '.');
                        sb.Insert(0, GetTypeFullName(parentTypeDeclSyntax));
                        parent = null;
                        break;
                    case NamespaceDeclarationSyntax namespaceSyntax:
                        sb.Insert(0, '.');
                        sb.Insert(0, namespaceSyntax.Name.ToString());
                        break;
                }

                parent = parent?.Parent;
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

        private static bool ContainsAttribute(AttributeSyntax attributeSyntax, IEnumerable<string> attrs)
        {
            var attrName = attributeSyntax.Name.ToString();
            return attrs.Contains(attrName);
        }

        private static bool ContainsAttribute(AttributeData attributeSyntax, IEnumerable<string> attrs)
        {
            var attrName = GetTypeFullName(attributeSyntax.AttributeClass!);
            return attrs.Contains(attrName);
        }

        private static IEnumerable<string> GetAttrNames(string attrFullName)
        {
            yield return attrFullName.Substring(0, attrFullName.Length - AttributeEnding.Length);
            yield return attrFullName;

            var dotIndex = attrFullName.LastIndexOf('.');
            if (dotIndex > 0)
            {
                var attrName = attrFullName.Substring(dotIndex + 1);
                yield return attrName.Substring(0, attrName.Length - AttributeEnding.Length);
                yield return attrName;
            }
        }
    }
}
