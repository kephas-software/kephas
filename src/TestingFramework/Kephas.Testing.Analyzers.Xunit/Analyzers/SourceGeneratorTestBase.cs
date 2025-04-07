// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceGeneratorTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

#if NET8_0
using AssemblyReferences = Basic.Reference.Assemblies.Net80.References;
#elif NET9_0
using AssemblyReferences = Basic.Reference.Assemblies.Net90.References;
#endif

namespace Kephas.Testing.Analyzers;

/// <summary>
/// Test base for source generators.
/// </summary>
/// <typeparam name="TSourceGenerator"></typeparam>
public abstract class SourceGeneratorTestBase<TSourceGenerator>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    /// <summary>
    /// Gets the <see cref="GetRunResult"/> for the source code.
    /// </summary>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="references">Optional. The assembly references.</param>
    /// <returns>The run result.</returns>
    protected GeneratorRunResult GetRunResult(string sourceCode, IEnumerable<Assembly>? references = null)
    {
        var compilation = CSharpCompilation.Create(
            GetType().Name,
            [CSharpSyntaxTree.ParseText(sourceCode)],
            GetReferences(references),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new TSourceGenerator();
        var sourceGenerator = generator.AsSourceGenerator();

        // trackIncrementalGeneratorSteps allows to report info about each step of the generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [sourceGenerator],
            driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));

        // Run the generator
        driver = driver.RunGenerators(compilation);

        // Assert the driver doesn't recompute the output
        return driver.GetRunResult().Results.Single();
    }

    private static IEnumerable<MetadataReference> GetReferences(IEnumerable<Assembly>? references)
    {
        var baseReferences = AssemblyReferences.All;
        return references is null
            ? baseReferences
            : baseReferences.Union(references.Select(a => AssemblyMetadata.CreateFromFile(a.Location).GetReference()));
    }
}