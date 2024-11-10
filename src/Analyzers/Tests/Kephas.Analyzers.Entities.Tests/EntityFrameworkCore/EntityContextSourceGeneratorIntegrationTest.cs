// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextSourceGeneratorIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Analyzers.Entities.TestAssembly;
using Kephas.Analyzers.EntityFrameworkCore;
using Kephas.Entities.EntityContexts.Generated;
using Kephas.Reflection;
using Kephas.Testing.Analyzers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kephas.Analyzers.Entities.Tests.EntityFrameworkCore;

public class EntityContextSourceGeneratorIntegrationTest : SourceGeneratorTestBase<EntityContextSourceGenerator>
{
    [Fact]
    public void EntityContextGenerated()
    {
        var testEntityContext = typeof(TestEntityContext);
        testEntityContext.GetPublicInstanceProperties()
            .Where(p => p.PropertyType.IsConstructedGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => p.PropertyType)
            .Should().BeEquivalentTo([typeof(DbSet<Person>), typeof(DbSet<Country>)]);
    }
}