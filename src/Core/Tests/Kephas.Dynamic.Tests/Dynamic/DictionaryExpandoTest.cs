// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExpandoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Dynamic;

namespace Kephas.Tests.Dynamic;

using Kephas.Dynamic;
using Xunit;

public class DictionaryExpandoTest
{
    [Fact]
    public void Constructor_object_dictionary()
    {
        var dict = new Dictionary<string, object?>();
        dynamic expando = new DictionaryExpando<object?>(dict);

        expando.Hi = "there";

        Assert.Equal("there", dict["Hi"]);
    }

    [Fact]
    public void Constructor_string_dictionary()
    {
        var dict = new Dictionary<string, string>();
        dynamic expando = new DictionaryExpando<string>(dict);

        expando.Hi = "there";

        Assert.Equal("there", dict["Hi"]);
    }

    [Fact]
    public void Dynamic_by_key()
    {
        dynamic expando = new DictionaryExpando<string>(new Dictionary<string, string> { { "Hi", "there" } });

        Assert.Equal("there", expando.Hi);
    }

    [Fact]
    public void GetKeys()
    {
        IDynamic expando = new DictionaryExpando<string>(new Dictionary<string, string> { { "Hi", "there" } });

        Assert.Equivalent(new[] { "Hi" }, expando.GetKeys());
    }

    [Fact]
    public void GetDynamicMemberNames()
    {
        DynamicObject expando = new DictionaryExpando<string>(new Dictionary<string, string> { { "Hi", "there" } });

        Assert.Equivalent(new[] { "Hi" }, expando.GetDynamicMemberNames());
    }
}
