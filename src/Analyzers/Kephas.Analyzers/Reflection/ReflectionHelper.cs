namespace Kephas.Analyzers.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class ReflectionHelper
{
    private static IEnumerable<string> factoryInterfaceIdentifiers = new[]
    {
        "IRuntimeElementInfoFactory",
        "IRuntimeTypeInfoFactory",
        "Kephas.Runtime.Factories.IRuntimeElementInfoFactory",
        "Kephas.Runtime.Factories.IRuntimeTypeInfoFactory",
    };

    public static bool IsFactoryInterface(TypeDeclarationSyntax type)
    {
        return factoryInterfaceIdentifiers.Contains(type.Identifier.Text);
    }

    public static bool IsFactoryInterface(INamedTypeSymbol type)
    {
        return factoryInterfaceIdentifiers.Contains(type.Name);
    }

    public static bool IsFactory(ClassDeclarationSyntax type, GeneratorExecutionContext context)
    {
        var semanticModel = context.Compilation.GetSemanticModel(type.SyntaxTree);
        var classSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(type)!;

        if (type.BaseList == null
            || type.Modifiers.Any(m => m.Text == "abstract")
            || type.Modifiers.Any(m => m.Text == "private")
            || type.TypeParameterList is not null)
        {
            if (type.Identifier.CanBeFactory())
            {
                context.ReportDiagnostic(
                    Diagnostic.Create("KEPREF101", "SourceGenerator",
                        $"{type.Identifier} was ignored when checking the IRuntimeElementInfoFactory classes",
                        DiagnosticSeverity.Warning, DiagnosticSeverity.Info, true, 1));
            }

            return false;
        }

        if (classSymbol.AllInterfaces.Any(IsFactoryInterface))
        {
            context.ReportDiagnostic(
                Diagnostic.Create("KEPREF102", "SourceGenerator",
                $"{type.Identifier} was found when checking the IRuntimeElementInfoFactory classes",
                DiagnosticSeverity.Info, DiagnosticSeverity.Info, true, 1));

            return true;
        }


        var baseType = classSymbol.BaseType;
        while (baseType != null)
        {
            if (IsFactory(baseType, context))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create("KEPREF102", "SourceGenerator",
                        $"{type.Identifier} was found when checking the IRuntimeElementInfoFactory classes",
                        DiagnosticSeverity.Info, DiagnosticSeverity.Info, true, 1));
                return true;
            }

            baseType = baseType.BaseType;
        }

        if (type.Identifier.CanBeFactory())
        {
            context.ReportDiagnostic(
                Diagnostic.Create("KEPREF101", "SourceGenerator",
                    $"{type.Identifier} was ignored when checking the IRuntimeElementInfoFactory classes",
                    DiagnosticSeverity.Warning, DiagnosticSeverity.Info, true, 1));
        }

        return false;
    }

    public static bool IsFactory(INamedTypeSymbol type, GeneratorExecutionContext context)
    {
        if (type.Interfaces.Any(IsFactoryInterface))
        {
            return true;
        }

        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (IsFactory(baseType, context))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    private static bool CanBeFactory(this SyntaxToken token)
    {
        return token.Text.EndsWith("InfoFactory");
    }
}