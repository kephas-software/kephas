// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalyzerServiceHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kephas.Analyzers.DependencyInjection;

/// <summary>
/// Helper class for injection source generators.
/// </summary>
internal static class DependencyInjectionAnalyzerHelper
{
    private const string AttributeEnding = "Attribute";
    private static readonly List<string> ExcludedAttrs;
    private static readonly List<string> ServiceAttrs;

    static DependencyInjectionAnalyzerHelper()
    {
        ExcludedAttrs = GetAttrNames("Kephas.DependencyInjection.ExcludeFromDependencyInjectionAttribute").ToList();
        ServiceAttrs = [];
        ServiceAttrs.AddRange(GetAttrNames("Kephas.DependencyInjection.TransientServiceAttribute"));
        ServiceAttrs.AddRange(GetAttrNames("Kephas.DependencyInjection.ScopedServiceAttribute"));
        ServiceAttrs.AddRange(GetAttrNames("Kephas.DependencyInjection.SingletonServiceAttribute"));
    }

    public static bool IsServiceDeclarationType(this TypeDeclarationSyntax type)
    {
        var attrLists = type.AttributeLists;
        var attrs = attrLists.SelectMany(al => al.Attributes).ToList();
        return !attrs.Any(a => ContainsAttribute(a, ExcludedAttrs))
               && attrs.Any(a => ContainsAttribute(a, ServiceAttrs))
               && type.Modifiers.Any(m => m.ValueText == "public")
               && type.Modifiers.All(m => m.ValueText != "static");
    }

    public static bool IsServiceDeclarationType(INamedTypeSymbol type)
    {
        var attrs = type.GetAttributes();
        return !attrs.Any(a => ContainsAttribute(a, ExcludedAttrs))
               && attrs.Any(a => ContainsAttribute(a, ServiceAttrs))
               && !type.IsStatic
               && type.DeclaredAccessibility == Accessibility.Public;
    }

    public static INamedTypeSymbol? TryGetServiceDeclarationType(ClassDeclarationSyntax type, IDeclareServicesSourceContext context)
    {
        var semanticModel = context.Compilation.GetSemanticModel(type.SyntaxTree);
        var classSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(type)!;

        if (IsServiceDeclarationType(type))
        {
            return GetOriginalServiceDeclarationType(classSymbol);
        }

        if (type.BaseList == null)
        {
            return null;
        }

        var baseInterfaceServiceType = classSymbol.AllInterfaces
            .FirstOrDefault(IsServiceDeclarationType);
        if (baseInterfaceServiceType is not null)
        {
            return GetOriginalServiceDeclarationType(baseInterfaceServiceType);
        }

        var baseType = classSymbol.BaseType;
        while (baseType != null)
        {
            if (IsServiceDeclarationType(baseType))
            {
                return GetOriginalServiceDeclarationType(baseType);
            }

            baseInterfaceServiceType = baseType.AllInterfaces
                .FirstOrDefault(IsServiceDeclarationType);
            if (baseInterfaceServiceType is not null)
            {
                return GetOriginalServiceDeclarationType(baseInterfaceServiceType);
            }

            baseType = baseType.BaseType;
        }

        return null;
    }

    private static INamedTypeSymbol GetOriginalServiceDeclarationType(INamedTypeSymbol namedTypeSymbol)
    {
        if (!namedTypeSymbol.IsGenericType)
        {
            return namedTypeSymbol;
        }

        var serviceAttr = namedTypeSymbol.GetAttributes()
            .FirstOrDefault(a => ContainsAttribute(a, ServiceAttrs));
        if (serviceAttr == null)
        {
            return namedTypeSymbol;
        }

        // for generic types, if a contract type is provided
        if (serviceAttr.NamedArguments.Any(a => a.Key == "ServiceType"))
        {
            var serviceType = serviceAttr.NamedArguments.First(a => a.Key == "ServiceType").Value.Value as INamedTypeSymbol;
            if (!(serviceType?.IsGenericType ?? false))
            {
                return namedTypeSymbol.ConstructedFrom ?? namedTypeSymbol;
            }
        }

        return namedTypeSymbol;
    }

    public static bool CanBeServiceImplementationService(this ClassDeclarationSyntax type)
    {
        var attrLists = type.AttributeLists;
        var attrs = attrLists.SelectMany(al => al.Attributes).ToList();
        if (attrs.Any(a => ContainsAttribute(a, ExcludedAttrs)))
        {
            return false;
        }

        return type.Modifiers.Any(m => m.ValueText is "public" or "internal")
               && type.Modifiers.All(m => m.ValueText != "static")
               && type.Modifiers.All(m => m.ValueText != "abstract");
    }

    public static bool CanBeMetadataType(this TypeDeclarationSyntax type)
    {
        if (type is not ClassDeclarationSyntax and not RecordDeclarationSyntax)
        {
            return false;
        }

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

    public static bool AppendServicesInfoProviderClass(
        (string typeNamespace, string typeName) serviceInfoProvider,
        StringBuilder source,
        IDeclareServicesSourceContext sourceContext,
        IList<ServiceImplementationDeclaration> serviceImplementationDeclarations)
    {
        var isProviderEmpty = true;

        source.AppendLine($@"namespace {serviceInfoProvider.typeNamespace}");
        source.AppendLine($@"{{");
        source.AppendLine($@"   [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]");
        source.AppendLine($@"   internal partial class {serviceInfoProvider.typeName}: IServiceInfoProvider");
        source.AppendLine($@"   {{");
        source.AppendLine($@"       IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()");
        source.AppendLine($@"       {{");

        var serviceDeclarationTypes = sourceContext.ServiceDeclarationTypes;
        var metadataTypes = sourceContext.MetadataTypes;
        if (serviceDeclarationTypes.Count > 0)
        {
            var contractTypesBuilder = new StringBuilder();
            foreach (var typeSyntax in serviceDeclarationTypes)
            {
                var typeFullName = typeSyntax.GetTypeFullName(sourceContext);
                var metadataTypeSyntax = TryGetMetadataType(typeSyntax, metadataTypes);
                var metadataTypeFullName = metadataTypeSyntax is null ? "null" : $"typeof({metadataTypeSyntax.GetTypeFullName(sourceContext)})";
                source.AppendLine($"            yield return new ServiceDeclaration(typeof({typeFullName}), {metadataTypeFullName});");
                contractTypesBuilder.Append($"{typeSyntax.Identifier}, ");
            }

            isProviderEmpty = false;

            contractTypesBuilder.Length -= 2;

            sourceContext.SourceProductionContext.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("KG1000", nameof(DeclareServicesSourceGenerator), $"Identified following application service contracts: {contractTypesBuilder}.", "Kephas", DiagnosticSeverity.Info, isEnabledByDefault: true), Location.None));
        }
        else
        {
            source.AppendLine($"            yield break;");
        }

        source.AppendLine($@"       }}");
        source.AppendLine();
        source.AppendLine($@"       IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)");
        source.AppendLine($@"       {{");

        if (serviceImplementationDeclarations.Count > 0)
        {
            var serviceTypesBuilder = new StringBuilder();
            foreach (var serviceImplementationDeclaration in serviceImplementationDeclarations)
            {
                var serviceDeclarationType = serviceImplementationDeclaration.ServiceDeclarationType;
                if (serviceDeclarationType == null)
                {
                    continue;
                }

                var typeFullName = serviceImplementationDeclaration.ImplementationType.GetTypeFullName(sourceContext);
                try
                {
                    source.AppendLine($"            yield return new ServiceImplementationDeclaration(typeof({typeFullName}), typeof({GetTypeFullName(serviceDeclarationType)}));");
                }
                catch (Exception ex)
                {
                    sourceContext.SourceProductionContext.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("KG2000", nameof(DeclareServicesSourceGenerator), $"Error while generating the service type for {typeFullName}: {ex.Message}", "Kephas", DiagnosticSeverity.Warning, isEnabledByDefault: true), Location.None));
                }

                serviceTypesBuilder.Append($"{serviceImplementationDeclaration.ImplementationType.Identifier}, ");

                isProviderEmpty = false;
            }

            if (serviceTypesBuilder.Length > 0)
            {
                serviceTypesBuilder.Length -= 2;

                sourceContext.SourceProductionContext.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("KG1000", nameof(DeclareServicesSourceGenerator), $"Identified following application service contracts: {serviceTypesBuilder}.", "Kephas", DiagnosticSeverity.Info, isEnabledByDefault: true), Location.None));
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

    private static TypeDeclarationSyntax? TryGetMetadataType(
        TypeDeclarationSyntax serviceDeclarationType,
        IEnumerable<TypeDeclarationSyntax> metadataTypes)
    {
        var metadataTypeName =
            serviceDeclarationType is InterfaceDeclarationSyntax && serviceDeclarationType.Identifier.Text.StartsWith("I")
                ? serviceDeclarationType.Identifier.Text.Substring(1)
                : serviceDeclarationType.Identifier.Text;
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