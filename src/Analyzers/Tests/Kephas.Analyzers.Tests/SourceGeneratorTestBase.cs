// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceGeneratorTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Kephas.Analyzers.Tests;

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
    /// <returns>The run result.</returns>
    protected GeneratorRunResult GetRunResult(string sourceCode)
    {
        var compilation = CSharpCompilation.Create(
            GetType().Name,
            [CSharpSyntaxTree.ParseText(sourceCode)],
            Basic.Reference.Assemblies.Net80.References.All,
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
}