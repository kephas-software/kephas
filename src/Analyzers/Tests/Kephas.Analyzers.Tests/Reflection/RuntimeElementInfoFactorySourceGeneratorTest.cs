// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeElementInfoFactorySourceGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Analyzers.AssemblyInitialization;
using Kephas.Analyzers.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Kephas.Analyzers.Tests.Reflection;

public class RuntimeElementInfoFactorySourceGeneratorTest
{
    [Fact]
    public void Generate_ReflectionInitializer()
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

                    public class MyRuntimeTypeInfoFactory : IRuntimeTypeInfoFactory
                    {
                    }

                    """)
            },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new RuntimeElementInfoFactorySourceGenerator();
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
                source.HintName.Should().Be("ReflectionInitializer.g.cs");
                var stringSource = source.SourceText.ToString();
                stringSource.Should().Contain(
                    """
                    using System;
                    using System.Collections.Generic;
                    using System.Diagnostics.CodeAnalysis;
                    using System.Reflection;
                    using System.Runtime.CompilerServices;
                    
                    using Kephas.Runtime;
                    using Kephas.Runtime.Factories;
                    
                    [assembly: AssemblyInitializer<Kephas.Reflection.Generated.ReflectionInitializer_TestProject>]
                    
                    namespace Kephas.Reflection.Generated
                    {
                        public class ReflectionInitializer_TestProject : IAssemblyInitializer
                        {
                            public void Initialize()
                            {
                               RuntimeTypeRegistry.Instance.RegisterFactory(new MyRuntimeTypeInfoFactory());
                            }
                        }
                    }
                    """);
            });
    }
}
