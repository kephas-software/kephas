// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextSourceGeneratorIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Analyzers.EntityFrameworkCore;
using Kephas.Testing.Analyzers;
using Xunit;

namespace Kephas.Analyzers.Tests.EntityFrameworkCore;

public class EntityContextSourceGeneratorIntegrationTest : SourceGeneratorTestBase<EntityContextSourceGenerator>
{
    [Fact]
    public void Generate_EntityContext_without_connectionName()
    {
        var sourceCode =
            """
            // --------------------------------------------------------------------------------------------------------------------
            // <copyright file="Entities.cs" company="Kephas Software SRL">
            //   Copyright (c) Kephas Software SRL. All rights reserved.
            //   Licensed under the MIT license. See LICENSE file in the project root for full license information.
            // </copyright>
            // --------------------------------------------------------------------------------------------------------------------
            
            using Kephas.Analyzers.Entities.TestAssembly;
            using Kephas.Entities;
            
            [assembly: GenerateEntityContext("Test", typeof(Person), typeof(Country))]
            
            namespace Kephas.Analyzers.Entities.TestAssembly;
            
            public record Person;
            public record Country;

            """;

        // Assert the driver doesn't recompute the output
        var result = GetRunResult(sourceCode);

        // Assert the driver use the cached result from AssemblyName and Syntax
        Assert.Collection(
            result.GeneratedSources,
            source =>
            {
                source.HintName.Should().Be("TestEntityContext.g.cs");
                var stringSource = source.SourceText.ToString().Replace("\r\n", "\n");
                stringSource.Should().Contain(
                    """
                    #nullable enable
                    
                    // Generated for EntityContextSourceGeneratorIntegrationTest, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
                    
                    using System.CodeDom.Compiler;
                    using Microsoft.EntityFrameworkCore;
                    using Kephas.DependencyInjection;
                    using Kephas.DependencyInjection.Metadata;
                    using Kephas.Entities;
                    using Kephas.Entities.Builder;
                    // --------------------------------------------------------------------------------------------------------------------
                    // <copyright file="Entities.cs" company="Kephas Software SRL">
                    //   Copyright (c) Kephas Software SRL. All rights reserved.
                    //   Licensed under the MIT license. See LICENSE file in the project root for full license information.
                    // </copyright>
                    // --------------------------------------------------------------------------------------------------------------------
                    
                    using Kephas.Analyzers.Entities.TestAssembly;
                    
                    namespace Kephas.Entities.EntityContexts.Generated;
                    
                    [Priority(Priority.Normal)]
                    [ConnectionName("Test")]
                    [EntityTypes(
                        typeof(Person)
                        , typeof(Country)
                    )]
                    [GeneratedCode("EntityContextSourceGenerator", "1.0.0.0")]
                    public partial class TestEntityContext : EntityContextBase<TestEntityContext>
                    {
                        public TestEntityContext(DbContextOptions<TestEntityContext> options, IEntityContextBuilder builder)
                            : base(options, builder)
                        {
                        }
                        public DbSet<Person> Person { get; set; } = null!;
                        public DbSet<Country> Country { get; set; } = null!;
                    }
                    
                    """.Replace("\r\n", "\n"));
            });
    }

    [Fact]
    public void Generate_EntityContext_with_connectionName()
    {
        var sourceCode =
            """
            // --------------------------------------------------------------------------------------------------------------------
            // <copyright file="Entities.cs" company="Kephas Software SRL">
            //   Copyright (c) Kephas Software SRL. All rights reserved.
            //   Licensed under the MIT license. See LICENSE file in the project root for full license information.
            // </copyright>
            // --------------------------------------------------------------------------------------------------------------------
            
            using Kephas.Analyzers.Entities.TestAssembly;
            using Kephas.Entities;
            
            [assembly: GenerateEntityContext("Test", "MyConnection", typeof(Person), typeof(Country))]
            
            namespace Kephas.Analyzers.Entities.TestAssembly;
            
            public record Person;
            public record Country;

            """;

        // Assert the driver doesn't recompute the output
        var result = GetRunResult(sourceCode);

        // Assert the driver use the cached result from AssemblyName and Syntax
        Assert.Collection(
            result.GeneratedSources,
            source =>
            {
                source.HintName.Should().Be("TestEntityContext.g.cs");
                var stringSource = source.SourceText.ToString().Replace("\r\n", "\n");
                stringSource.Should().Contain(
                    """
                    #nullable enable
                    
                    // Generated for EntityContextSourceGeneratorIntegrationTest, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
                    
                    using System.CodeDom.Compiler;
                    using Microsoft.EntityFrameworkCore;
                    using Kephas.DependencyInjection;
                    using Kephas.DependencyInjection.Metadata;
                    using Kephas.Entities;
                    using Kephas.Entities.Builder;
                    // --------------------------------------------------------------------------------------------------------------------
                    // <copyright file="Entities.cs" company="Kephas Software SRL">
                    //   Copyright (c) Kephas Software SRL. All rights reserved.
                    //   Licensed under the MIT license. See LICENSE file in the project root for full license information.
                    // </copyright>
                    // --------------------------------------------------------------------------------------------------------------------
                    
                    using Kephas.Analyzers.Entities.TestAssembly;
                    
                    namespace Kephas.Entities.EntityContexts.Generated;
                    
                    [Priority(Priority.Normal)]
                    [ConnectionName("MyConnection")]
                    [EntityTypes(
                        typeof(Person)
                        , typeof(Country)
                    )]
                    [GeneratedCode("EntityContextSourceGenerator", "1.0.0.0")]
                    public partial class TestEntityContext : EntityContextBase<TestEntityContext>
                    {
                        public TestEntityContext(DbContextOptions<TestEntityContext> options, IEntityContextBuilder builder)
                            : base(options, builder)
                        {
                        }
                        public DbSet<Person> Person { get; set; } = null!;
                        public DbSet<Country> Country { get; set; } = null!;
                    }
                    
                    """.Replace("\r\n", "\n"));
            });
    }
}