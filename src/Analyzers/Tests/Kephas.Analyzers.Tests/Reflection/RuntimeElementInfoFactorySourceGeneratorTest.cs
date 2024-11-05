// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeElementInfoFactorySourceGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Analyzers.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Kephas.Analyzers.Tests.Reflection;

public class RuntimeElementInfoFactorySourceGeneratorTest : SourceGeneratorTestBase<RuntimeElementInfoFactorySourceGenerator>
{
    [Fact]
    public void Generate_ReflectionInitializer()
    {
        var sourceCode =
            """

            using System;
            using Kephas.Runtime;

            namespace Kephas.Sample;

            public class MyRuntimeTypeInfoFactory : IRuntimeTypeInfoFactory
            {
            }

            """;

        // Assert the driver doesn't recompute the output
        var result = GetRunResult(sourceCode);

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
                    
                    [assembly: AssemblyInitializer<Kephas.Reflection.Generated.ReflectionInitializer_RuntimeElementInfoFactorySourceGeneratorTest>]
                    
                    namespace Kephas.Reflection.Generated
                    {
                        public class ReflectionInitializer_RuntimeElementInfoFactorySourceGeneratorTest : IAssemblyInitializer
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
