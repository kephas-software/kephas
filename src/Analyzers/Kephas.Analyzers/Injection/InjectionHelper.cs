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
    using Kephas.Injection.AttributedModel;
    using Kephas.Services;
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
            ExcludedAttrs = GetAttrNames(typeof(ExcludeFromInjectionAttribute)).ToList();
            AppServiceContractAttrs = new List<string>();
            AppServiceContractAttrs.AddRange(GetAttrNames(typeof(AppServiceContractAttribute)));
            AppServiceContractAttrs.AddRange(GetAttrNames(typeof(SingletonAppServiceContractAttribute)));
            AppServiceContractAttrs.AddRange(GetAttrNames(typeof(ScopedAppServiceContractAttribute)));
        }

        public static bool IsAppServiceContract(TypeDeclarationSyntax type)
        {
            var attrLists = type.AttributeLists;
            var attrs = attrLists.SelectMany(al => al.Attributes).ToList();
            return !attrs.Any(a => ContainsAttribute(a, ExcludedAttrs))
                   && attrs.Any(a => ContainsAttribute(a, AppServiceContractAttrs))
                   && type.Modifiers.Any(m => m.ValueText == "public");
        }

        public static bool CanBeAppService(TypeDeclarationSyntax type)
        {
            var attrLists = type.AttributeLists;
            var attrs = attrLists.SelectMany(al => al.Attributes).ToList();
            if (attrs.Any(a => ContainsAttribute(a, ExcludedAttrs)))
            {
                return false;
            }

            // TODO public and not abstract

            return type.Modifiers.Any(m => m.ValueText == "public");
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

        private static IEnumerable<string> GetAttrNames(Type attributeType)
        {
            yield return attributeType.Name;
            yield return attributeType.FullName;
            yield return attributeType.Name.Substring(0, attributeType.Name.Length - AttributeEnding.Length);
            yield return attributeType.FullName!.Substring(0, attributeType.FullName.Length - AttributeEnding.Length);
        }
    }
}
