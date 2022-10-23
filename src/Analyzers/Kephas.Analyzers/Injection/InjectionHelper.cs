// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

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
            ExcludedAttrs = GetAttrNames("Kephas.Services.AttributedModel.ExcludeFromInjectionAttribute").ToList();
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
                return GetOriginalAppServiceContract(classSymbol);
            }

            if (type.BaseList == null)
            {
                return null;
            }

            foreach (var baseInterface in classSymbol.AllInterfaces)
            {
                if (IsAppServiceContract(baseInterface))
                {
                    return GetOriginalAppServiceContract(baseInterface);
                }
            }

            var baseType = classSymbol.BaseType;
            while (baseType != null)
            {
                if (IsAppServiceContract(baseType))
                {
                    return GetOriginalAppServiceContract(baseType);
                }

                baseType = baseType.BaseType;
            }

            return null;
        }

        private static INamedTypeSymbol GetOriginalAppServiceContract(INamedTypeSymbol namedTypeSymbol)
        {
            if (!namedTypeSymbol.IsGenericType)
            {
                return namedTypeSymbol;
            }

            var contractAttr = namedTypeSymbol.GetAttributes()
                .FirstOrDefault(a => ContainsAttribute(a, AppServiceContractAttrs));
            if (contractAttr == null)
            {
                return namedTypeSymbol;
            }

            // for generic types, if a contract type is provided
            if (contractAttr.NamedArguments.Any(a => a.Key == "ContractType"))
            {
                var contractType = contractAttr.NamedArguments.First(a => a.Key == "ContractType").Value.Value as INamedTypeSymbol;
                if (!(contractType?.IsGenericType ?? false))
                {
                    return namedTypeSymbol.ConstructedFrom ?? namedTypeSymbol;
                }
            }

            return namedTypeSymbol;
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

        public static bool CanBeMetadataType(ClassDeclarationSyntax type)
        {
            var attrLists = type.AttributeLists;
            var attrs = attrLists.SelectMany(al => al.Attributes).ToList();
            if (attrs.Any(a => ContainsAttribute(a, ExcludedAttrs)))
            {
                return false;
            }

            return type.Modifiers.Any(m => m.ValueText == "public")
                   && type.Modifiers.All(m => m.ValueText != "static")
                   && type.Modifiers.All(m => m.ValueText != "abstract")
                   && type.Identifier.Text.EndsWith("Metadata");
        }

        public static string GetTypeFullName(INamedTypeSymbol typeSymbol)
        {
            var fullNameBuilder = new StringBuilder(typeSymbol.Name);
            var ns = typeSymbol.ContainingNamespace;
            while (ns != null)
            {
                if (!string.IsNullOrEmpty(ns.Name))
                {
                    fullNameBuilder.Insert(0, ns.Name + ".");
                }

                ns = ns.ContainingNamespace;
            }

            var isBoundGenericType = false;
            var isUnboundGenericType = false;
            if (typeSymbol.IsUnboundGenericType)
            {
                fullNameBuilder.Append('<');
                for (var i = 0; i < typeSymbol.TypeParameters.Length - 1; i++)
                {
                    fullNameBuilder.Append(',');
                }

                fullNameBuilder.Append('>');
            }
            else if (typeSymbol.IsGenericType)
            {
                fullNameBuilder.Append('<');
                foreach (var typeArg in typeSymbol.TypeArguments)
                {
                    if (typeArg is INamedTypeSymbol namedTypeArg)
                    {
                        fullNameBuilder.Append(GetTypeFullName(namedTypeArg));
                        isBoundGenericType = true;
                    }
                    else
                    {
                        isUnboundGenericType = true;
                    }

                    fullNameBuilder.Append(", ");
                }

                fullNameBuilder.Length -= 2;
                fullNameBuilder.Append('>');
            }

            return isBoundGenericType && isUnboundGenericType
                ? throw new InvalidOperationException($"Generic type '{fullNameBuilder}' is partially bound, while it should be either unbound or fully bound. Possible reason: the [AppServiceContract] attribute is applied on a base type instead of the type itself. Check also the 'AsOpenGeneric' option.")
                : fullNameBuilder.ToString();
        }

        public static bool AppendAppServicesProviderClass(
            (string typeNamespace, string typeName) serviceTypeProvider,
            StringBuilder source,
            GeneratorExecutionContext context,
            IList<ServiceDeclaration> serviceTypes,
            IAppServicesCompilationContext compilationContext)
        {
            var isProviderEmpty = true;

            source.AppendLine($@"namespace {serviceTypeProvider.typeNamespace}");
            source.AppendLine($@"{{");
            source.AppendLine($@"#if NET6_0_OR_GREATER");
            source.AppendLine($@"   [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]");
            source.AppendLine($@"#endif");
            source.AppendLine($@"   internal partial class {serviceTypeProvider.typeName}: IAppServiceInfosProvider");
            source.AppendLine($@"   {{");
            source.AppendLine($@"       IEnumerable<ContractDeclarationInfo>? IAppServiceInfosProvider.GetContractDeclarationTypes()");
            source.AppendLine($@"       {{");

            var contractTypes = compilationContext.ContractTypes;
            var metadataTypes = compilationContext.MetadataTypes;
            if (contractTypes.Count > 0)
            {
                var contractTypesBuilder = new StringBuilder();
                foreach (var typeSyntax in contractTypes)
                {
                    var typeFullName = typeSyntax.GetTypeFullName(compilationContext);
                    var metadataTypeSyntax = InjectionHelper.TryGetMetadataType(typeSyntax, metadataTypes);
                    var metadataTypeFullName = metadataTypeSyntax is null ? "null" : $"typeof({metadataTypeSyntax.GetTypeFullName(compilationContext)})";
                    source.AppendLine($"            yield return new ContractDeclarationInfo(typeof({typeFullName}), {metadataTypeFullName});");
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
            source.AppendLine($@"       IEnumerable<ServiceDeclaration> IAppServiceInfosProvider.GetAppServices()");
            source.AppendLine($@"       {{");

            if (serviceTypes.Count > 0)
            {
                var serviceTypesBuilder = new StringBuilder();
                foreach (var serviceDeclaration in serviceTypes)
                {
                    var appServiceContract = serviceDeclaration.ContractType;
                    if (appServiceContract == null)
                    {
                        continue;
                    }

                    var typeFullName = serviceDeclaration.ServiceType.GetTypeFullName(compilationContext);
                    try
                    {
                        source.AppendLine($"            yield return new ServiceDeclaration(typeof({typeFullName}), typeof({InjectionHelper.GetTypeFullName(appServiceContract)}));");
                    }
                    catch (Exception ex)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("KG2000", nameof(AppServicesSourceGenerator), $"Error while generating the service type for {typeFullName}: {ex.Message}", "Kephas", DiagnosticSeverity.Warning, isEnabledByDefault: true), Location.None));
                    }

                    serviceTypesBuilder.Append($"{serviceDeclaration.ServiceType.Identifier}, ");

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

        private static ClassDeclarationSyntax? TryGetMetadataType(
            TypeDeclarationSyntax contractType,
            IEnumerable<ClassDeclarationSyntax> metadataTypes)
        {
            var metadataTypeName =
                contractType is InterfaceDeclarationSyntax && contractType.Identifier.Text.StartsWith("I")
                    ? contractType.Identifier.Text.Substring(1)
                    : contractType.Identifier.Text;
            metadataTypeName = $"{metadataTypeName}Metadata";
            return metadataTypes.FirstOrDefault(t => t.Identifier.Text == metadataTypeName);
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
