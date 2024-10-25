// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyLoaderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Collections;
using Kephas.Logging;
using Kephas.Runtime;
using Xunit;

namespace Kephas.Tests.Runtime;

public class AssemblyLoaderTest
{
    [Theory]
    [InlineData(new[] { typeof(IHasLogLevel) }, new[] { "Kephas.Extensions" })]
    [InlineData(new[] { typeof(IDictionaryConvertible) }, new[] { "Kephas.Collections" })]
    [InlineData(new[] { typeof(IHasLogLevel), typeof(IDictionaryConvertible) }, new[] { "Kephas.Extensions", "Kephas.Collections" })]
    [InlineData(new[] { typeof(IAssemblyLoader) }, new[] { "Kephas.Runtime.AssemblyLoader", "Kephas.Collections" })]
    public void FlattenReferences_assembly_enumeration(IEnumerable<Type> assemblyTypes, IEnumerable<string> expectedNames)
    {
        var loader = new AssemblyLoader();
        var assemblies = assemblyTypes
            .Select(t => t.Assembly);
        var assemblyNames = loader.FlattenReferences(assemblies, n => n.Name?.StartsWith("Kephas") is true)
            .Select(a => a.GetName()?.Name);
        
        assemblyNames.Should().BeEquivalentTo(expectedNames);
    }
}
