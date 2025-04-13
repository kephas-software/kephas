// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializerSourceGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Analyzers.AssemblyInitialization;
using Kephas.Testing.Analyzers;
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
                stringSource.Should().Contain("""
                        #nullable enable
                        
                        using System;
                        using System.Collections.Generic;
                        using System.Diagnostics.CodeAnalysis;
                        using System.Reflection;
                        using System.Runtime.CompilerServices;
                        
                        using Kephas.Runtime;
                        
                        namespace ModuleInitializerSourceGeneratorTest.AssemblyInitialization.Generated
                        {
                            internal static class ModuleInitializer_ModuleInitializerSourceGeneratorTest
                            {
                                [ModuleInitializer]
                                internal static void InitializeModule()
                                {
                                    IAssemblyInitializer initializer;
                                    initializer = (IAssemblyInitializer)new MyAssemblyInitializer();
                                    initializer.InitializeAssembly();
                                    var thisAssembly = typeof(ModuleInitializer_ModuleInitializerSourceGeneratorTest).Assembly;
                                    foreach (var assemblyInitializer in thisAssembly.GetCustomAttributes<Attribute>().OfType<IAssemblyInitializer>())
                                    {
                                        try 
                                        {
                                            assemblyInitializer.InitializeAssembly();
                                        }
                                        catch (Exception ex)
                                        {
                                            // write exception somewhere...
                                        }
                                    }
                                }
                            }
                        }
                        
                        """);
            });
    }
}
