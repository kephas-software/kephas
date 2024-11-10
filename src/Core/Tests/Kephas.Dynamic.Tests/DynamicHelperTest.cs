// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicHelperTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Dynamic;
using FluentAssertions;
using Kephas.Dynamic;
using NSubstitute;
using Xunit;

namespace Kephas.Tests;

public class DynamicHelperTest
{
    [Fact]
    public void ToDynamicObject_dynamic()
    {
        var obj = new ExpandoObject();
        var dyn = obj.ToDynamicObject();
        Assert.Same(obj, dyn);
    }

    [Fact]
    public void ToDynamicObject_non_dynamic_method()
    {
        var obj = new List<string>();
        var dyn = obj.ToDynamicObject();
        Assert.NotSame(obj, dyn);

        dyn.Add("John");
        obj.Should().HaveCount(1);
        obj.Should().Contain("John");
    }

    [Fact]
    public void ToDynamicObject_non_dynamic_property()
    {
        var obj = new List<string> { "one", "two" };
        var dyn = obj.ToDynamicObject();
        Assert.NotSame(obj, dyn);
        2.Should().Be(dyn.Count);
    }

    [Fact]
    public void ToDynamicObject_null_exception()
    {
        Action act = () => ((object)null).ToDynamicObject();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToDynamic_null_exception()
    {
        Action act = () => ((object)null!).ToDynamic();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToDynamic_object()
    {
        var obj = new { Name = "John", FamilyName = "Doe" };
        var expando = obj.ToDynamic();

        expando.Should().NotBeSameAs(obj);
        expando["Name"].Should().Be("John");
        expando["FamilyName"].Should().Be("Doe");
    }

    [Fact]
    public void ToDynamic_dynamic()
    {
        var expando = Substitute.For<IDynamic>();
        expando.ToDynamic().Should().BeSameAs(expando);
    }

    [Fact]
    public void ToDynamic_dictionary_string_object()
    {
        var dictionary = new Dictionary<string, object?>();
        var expando = dictionary.ToDynamic();

        expando["hi"] = "there";

        dictionary["hi"].Should().Be("there");
    }

    [Fact]
    public void ToDynamic_dictionary_string_string()
    {
        var dictionary = new Dictionary<string, string>();
        var expando = dictionary.ToDynamic();

        expando["hi"] = "there";

        dictionary["hi"].Should().Be("there");
    }
}