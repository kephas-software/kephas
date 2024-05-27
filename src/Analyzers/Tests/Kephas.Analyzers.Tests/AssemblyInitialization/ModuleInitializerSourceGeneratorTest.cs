// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializerSourceGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Analyzers.AssemblyInitialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Kephas.Analyzers.Tests.AssemblyInitialization;

public class ModuleInitializerSourceGeneratorTest
{
    [Fact]
    public void Generate_MyAssemblyInitializer()
    {
        var compilation = CSharpCompilation.Create(
            "TestProject",
            new[]
            {
                CSharpSyntaxTree.ParseText(
                    """

                    using System;
                    using Kephas.Runtime;

                    namespace Kephas.Sample;

                    public class MyAssemblyInitializer : IAssemblyInitializer
                    {
                        public void InitializeAssembly() { }
                    }

                    """)
            },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new ModuleInitializerSourceGenerator();
        var sourceGenerator = generator.AsSourceGenerator();

        // trackIncrementalGeneratorSteps allows to report info about each step of the generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: new[] { sourceGenerator },
            driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));

        // Run the generator
        driver = driver.RunGenerators(compilation);

        // Assert the driver doesn't recompute the output
        var result = driver.GetRunResult().Results.Single();

        // Assert the driver use the cached result from AssemblyName and Syntax
        Assert.Collection(
            result.GeneratedSources,
            source =>
            {
                source.HintName.Should().Be("ModuleInitializer.g.cs");
                var stringSource = source.SourceText.ToString();
                stringSource.Should().Contain("[ModuleInitializer]");
                stringSource.Should().Contain("internal static class ModuleInitializer_TestProject");
                stringSource.Should().Contain("initializer = (IAssemblyInitializer)new MyAssemblyInitializer();");
                stringSource.Should().Contain("initializer.InitializeAssembly();");
            });
    }
}
