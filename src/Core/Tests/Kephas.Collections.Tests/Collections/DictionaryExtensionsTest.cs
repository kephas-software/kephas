// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Xunit;

namespace Kephas.Tests.Collections;

public class DictionaryExtensionsTest
{
    [Fact]
    public void Merge_different()
    {
        var dict = new Dictionary<string, string> { { "1", "one" } }
            .Merge(new Dictionary<string, string>() { { "2", "two" } });

        Assert.Equal(2, dict.Count);
        Assert.Equal("one", dict["1"]);
        Assert.Equal("two", dict["2"]);
    }

    [Fact]
    public void Merge_same_key()
    {
        var dict = new Dictionary<string, string> { { "1", "one" }, { "2", "two" } }
            .Merge(new Dictionary<string, string>() { { "2", "three" } });

        Assert.Equal(2, dict.Count);
        Assert.Equal("one", dict["1"]);
        Assert.Equal("three", dict["2"]);
    }
}