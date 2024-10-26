// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInitializerAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Runtime;
using Xunit;

namespace Kephas.Tests.Runtime;

public class AssemblyInitializerAttributeTest
{
    private static bool initializerCalled = false;
    
    [Fact]
    public void InitializeAssembly()
    {
        var attr = new AssemblyInitializerAttribute<TestAttributeInitializer>();
        initializerCalled.Should().BeFalse();
        attr.InitializeAssembly();
        initializerCalled.Should().BeTrue();
    }

    internal class TestAttributeInitializer : IAssemblyInitializer
    {
        public void InitializeAssembly()
        {
            initializerCalled = true;
        }
    }
}