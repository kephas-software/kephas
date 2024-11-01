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

public class ModuleInitializerSourceGeneratorTest : SourceGeneratorTestBase<ModuleInitializerSourceGenerator>
{
    [Fact]
    public void Generate_MyAssemblyInitializer()
    {
        var sourceCode =
            """

            using System;
            using Kephas.Runtime;

            namespace Kephas.Sample;

            public class MyAssemblyInitializer : IAssemblyInitializer
            {
                public void InitializeAssembly() { }
            }

            """;

        var result = GetRunResult(sourceCode);

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
