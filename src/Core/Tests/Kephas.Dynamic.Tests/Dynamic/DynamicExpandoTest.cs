// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicExpandoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Dynamic;

using Kephas.Dynamic;
using Xunit;

public class DynamicExpandoTest
{
    [Fact]
    public void Indexer()
    {
        dynamic obj = new DictionaryExpando<object?>(new Dictionary<string, object?> { { "name", "John Doe" } });
        var expando = new DynamicExpando(obj);

        Assert.Equal("John Doe", expando["name"]);
    }

    [Fact]
    public void GetKeys()
    {
        dynamic obj = new DictionaryExpando<string>(new Dictionary<string, string> { { "Hi", "there" } });
        var expando = new DynamicExpando(obj);

        Assert.Equivalent(new[] { "Hi" }, expando.GetKeys());
    }
}
