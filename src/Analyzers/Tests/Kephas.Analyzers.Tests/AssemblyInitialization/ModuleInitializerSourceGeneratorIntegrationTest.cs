// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializerSourceGeneratorIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Kephas.Analyzers.TestAssembly;
using Xunit;

namespace Kephas.Analyzers.Tests.AssemblyInitialization;

public class ModuleInitializerSourceGeneratorIntegrationTest
{
    [Fact]
    public void Execute_generated_ModuleInitializer()
    {
        var testAssembly = typeof(NoService).Assembly;

        var moduleInitializerType = testAssembly.GetTypes()
            .SingleOrDefault(t => t.Name == "ModuleInitializer_Kephas_Analyzers_TestAssembly");
        moduleInitializerType.Should().NotBeNull();
        moduleInitializerType!.Namespace.Should().Be("Kephas.Analyzers.TestAssembly.AssemblyInitialization.Generated");

        var initializeMethod = moduleInitializerType
            .GetMethod("InitializeModule", BindingFlags.NonPublic | BindingFlags.Static);
        initializeMethod.Should().NotBeNull();
        initializeMethod!.IsStatic.Should().BeTrue();
        initializeMethod.GetCustomAttribute<ModuleInitializerAttribute>().Should().NotBeNull();
    }
}