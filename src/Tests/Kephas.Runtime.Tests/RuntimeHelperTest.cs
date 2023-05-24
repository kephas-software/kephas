// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeHelperTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Commands;
using Kephas.Dynamic;
using Kephas.ExceptionHandling;
using Kephas.Logging;
using NUnit.Framework;

namespace Kephas.Tests;

[TestFixture]
public class RuntimeHelperTest
{
    [Test]
    [TestCase(new[] { typeof(RuntimeHelper) }, new[] { "Kephas.Runtime", "Kephas.Commands.Abstractions", "Kephas.Core", "Kephas.Runtime.Abstractions", "Kephas.Dynamic", "Kephas.Exceptions", "Kephas.IO", "Kephas.Logging.Abstractions", "Kephas.Services", "Kephas.Services.Abstractions", "Kephas.Threading", "Kephas.Versioning" })]
    [TestCase(new[] { typeof(IArgs) }, new[] { "Kephas.Commands.Abstractions", "Kephas.Dynamic", "Kephas.Core", "Kephas.Exceptions", "Kephas.Logging.Abstractions" })]
    [TestCase(new[] { typeof(IAdapter) }, new[] { "Kephas.Core", "Kephas.Exceptions", "Kephas.Logging.Abstractions" })]
    [TestCase(new[] { typeof(ExceptionData) }, new[] { "Kephas.Exceptions", "Kephas.Logging.Abstractions" })]
    [TestCase(new[] { typeof(ILogger) }, new[] { "Kephas.Logging.Abstractions" })]
    [TestCase(new[] { typeof(IExpando) }, new[] { "Kephas.Dynamic" })]
    [TestCase(new[] { typeof(IAdapter), typeof(IExpando) }, new[] { "Kephas.Core", "Kephas.Dynamic", "Kephas.Exceptions", "Kephas.Logging.Abstractions" })]
    public void FlattenReferences_assembly_enumeration(IEnumerable<Type> assemblyTypes, IEnumerable<string> assemblyNames)
    {
        var assemblies = assemblyTypes
            .Select(t => t.Assembly)
            .FlattenReferences(n => n.Name?.StartsWith("Kephas") is true)
            .Select(a => a.GetName()?.Name);
        
        CollectionAssert.AreEquivalent(assemblies, assemblyNames);
    }
    
    [Test]
    [TestCase(typeof(RuntimeHelper), new[] { "Kephas.Commands.Abstractions", "Kephas.Core", "Kephas.Runtime", "Kephas.Runtime.Abstractions", "Kephas.Dynamic", "Kephas.Exceptions", "Kephas.IO", "Kephas.Logging.Abstractions", "Kephas.Services", "Kephas.Services.Abstractions", "Kephas.Threading", "Kephas.Versioning" })]
    [TestCase(typeof(IArgs), new[] { "Kephas.Commands.Abstractions", "Kephas.Dynamic", "Kephas.Core", "Kephas.Exceptions", "Kephas.Logging.Abstractions" })]
    [TestCase(typeof(IAdapter), new[] { "Kephas.Core", "Kephas.Exceptions", "Kephas.Logging.Abstractions" })]
    [TestCase(typeof(ExceptionData), new[] { "Kephas.Exceptions", "Kephas.Logging.Abstractions" })]
    [TestCase(typeof(ILogger), new[] { "Kephas.Logging.Abstractions" })]
    [TestCase(typeof(IExpando), new[] { "Kephas.Dynamic" })]
    public void FlattenReferences_assembly(Type assemblyType, IEnumerable<string> assemblyNames)
    {
        var assemblies = assemblyType.Assembly
            .FlattenReferences(n => n.Name?.StartsWith("Kephas") is true)
            .Select(a => a.GetName()?.Name);
        
        CollectionAssert.AreEquivalent(assemblies, assemblyNames);
    }
}