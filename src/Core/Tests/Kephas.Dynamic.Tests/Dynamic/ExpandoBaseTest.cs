// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the expando base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Dynamic;

using System.Collections.Generic;
using System.Linq;
using Kephas.Dynamic;
using Xunit;

public class ExpandoBaseTest
{
    [Fact]
    public void GetKeys_no_duplicates()
    {
        var expando = new TestExpando(new Named());

        var members = expando.GetKeys().ToList();
        Assert.Equal(1, members.Count);
        Assert.Equal("Name", members[0]);
    }

    [Fact]
    public void GetKeys_this_binders()
    {
        var expando = new TestExpando(new object(), binders: ExpandoMemberBinderKind.This);
        expando["Name"] = "gigi";
        expando["FamilyName"] = "belogea";

        var members = expando.GetKeys().ToList();
        Assert.Equal(1, members.Count);
        Assert.Equal("Name", members[0]);
    }

    [Fact]
    public void GetKeys_inner_object_binders()
    {
        var obj = new Named();
        var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerObject);
        expando["Name"] = "gigi";
        expando["FamilyName"] = "belogea";

        var members = expando.GetKeys().ToList();
        Assert.Equal(1, members.Count);
        Assert.Equal("Name", members[0]);
    }

    [Fact]
    public void GetKeys_inner_dictionary_binders()
    {
        var obj = new Named();
        var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerDictionary);
        expando["FamilyName"] = "belogea";

        var members = expando.GetKeys().ToList();
        Assert.Equal(1, members.Count);
        Assert.Equal("FamilyName", members[0]);
    }

    [Fact]
    public void HasDynamicMember_this_binders()
    {
        var expando = new TestExpando(new object(), binders: ExpandoMemberBinderKind.This);
        expando["Name"] = "gigi";
        expando["FamilyName"] = "belogea";

        Assert.True(expando.HasKey("Name"));
        Assert.False(expando.HasKey("FamilyName"));
    }

    [Fact]
    public void HasDynamicMember_inner_object_binders()
    {
        var obj = new Named();
        var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerObject);
        expando["Name"] = "gigi";
        expando["FamilyName"] = "belogea";

        Assert.True(expando.HasKey("Name"));
        Assert.False(expando.HasKey("FamilyName"));
    }

    [Fact]
    public void HasDynamicMember_inner_dictionary_binders()
    {
        var obj = new Named();
        var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerDictionary);
        expando["FamilyName"] = "belogea";

        Assert.False(expando.HasKey("Name"));
        Assert.True(expando.HasKey("FamilyName"));
    }

    [Fact]
    public void ToDictionary_this_binders()
    {
        var expando = new TestExpando(new object(), binders: ExpandoMemberBinderKind.This);
        expando["Name"] = "gigi";
        expando["FamilyName"] = "belogea";

        var dictionary = expando.ToDictionary();
        Assert.Single(dictionary);
        Assert.Equal("gigi", dictionary["Name"]);
        Assert.Equal("gigi", expando.Name);
        Assert.False(expando.GetInnerDictionary().ContainsKey("Name"));
    }

    [Fact]
    public void ToDictionary_inner_object_binders()
    {
        var obj = new Named();
        var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerObject);
        expando["Name"] = "gigi";
        expando["FamilyName"] = "belogea";

        var dictionary = expando.ToDictionary();
        Assert.Equal(1, dictionary.Count);
        Assert.Equal("gigi", dictionary["Name"]);
        Assert.Equal("gigi", obj.Name);
        Assert.Null(expando.Name);
    }

    [Fact]
    public void ToDictionary_inner_dictionary_binders()
    {
        var obj = new Named();
        var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerDictionary);
        expando["Name"] = "gigi";
        expando["FamilyName"] = "belogea";

        var dictionary = expando.ToDictionary();
        Assert.Equal(2, dictionary.Count);
        Assert.Equal("gigi", dictionary["Name"]);
        Assert.Equal("belogea", dictionary["FamilyName"]);
        Assert.Null(obj.Name);
        Assert.Null(expando.Name);
    }

    [Fact]
    public void ToDictionary_duplicated_property()
    {
        var dict = new Dictionary<string, object>();
        var expando = new TestExpando(dict);
        dict["Name"] = "gigi";
        expando.Name = "belogea";

        var dictionary = expando.ToDictionary();
        Assert.Equal("belogea", dictionary["Name"]);
    }

    [Fact]
    public void ToDictionary_wrapped_duplicated_property()
    {
        var dict = new Dictionary<string, object>();
        var obj = new Named();
        var expando = new TestExpando(obj, dict);
        dict["Name"] = "gigi";
        expando.Name = "belogea";
        obj.Name = "rules";

        var dictionary = expando.ToDictionary();
        Assert.Equal("belogea", dictionary["Name"]);
    }

    [Fact]
    public void ToDictionary_transformed_key()
    {
        var dict = new Dictionary<string, object>();
        var expando = new TestExpando(dict);
        dict["Name"] = "gigi";
        expando.Name = "belogea";

        var dictionary = expando.ToDictionary(k => k.ToLower());
        Assert.Equal("belogea", dictionary["name"]);
    }

    [Fact]
    public void ToDictionary_transformed_value()
    {
        var dict = new Dictionary<string, object>();
        var expando = new TestExpando(dict);
        dict["Name"] = "gigi";
        expando.Name = "belogea";

        var dictionary = expando.ToDictionary(valueGetter: v => v is string stringValue ? stringValue.ToUpper() : v);
        Assert.Equal("BELOGEA", dictionary["Name"]);
    }

    [Fact]
    public void ToDictionary_transformed_key_and_value()
    {
        var dict = new Dictionary<string, object>();
        var expando = new TestExpando(dict);
        dict["Name"] = "gigi";
        expando.Name = "belogea";

        var dictionary = expando.ToDictionary(k => k.ToLower(), v => v is string stringValue ? stringValue.ToUpper() : v);
        Assert.Equal("BELOGEA", dictionary["name"]);
    }

    [Fact]
    public void Indexer_non_conflicting_with_constant()
    {
        var dict = new Dictionary<string, object>();
        var expando = new TestExpandoWithConstants(dict);
        expando[nameof(TestExpandoWithConstants.Constant)] = "something";

        Assert.Equal("something", expando[nameof(TestExpandoWithConstants.Constant)]);
    }
}

public class TestExpandoWithConstants : ExpandoBase<object?>
{
    public const string Constant = "gigi";

    public TestExpandoWithConstants(object inner, IDictionary<string, object?>? innerDictionary = null)
        : base(inner, innerDictionary)
    {
    }

    public string Name { get; set; }
}

public class TestExpando : ExpandoBase<object?>
{
    public TestExpando(object inner, IDictionary<string, object?>? innerDictionary = null, ExpandoMemberBinderKind? binders = null)
        : base(inner, innerDictionary)
    {
        if (binders != null)
        {
            this.MemberBinders = binders.Value;
        }
    }

    public string Name { get; set; }

    public IDictionary<string, object?> GetInnerDictionary() => this.InnerDictionary;
}

public class TestExpandoNonConflicting : ExpandoBase<object?>
{
    public TestExpandoNonConflicting(object inner, IDictionary<string, object?>? innerDictionary = null)
        : base(inner, innerDictionary)
    {
    }
}

public class Named
{
    public string? Name { get; set; }
}