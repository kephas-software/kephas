// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExpandoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Dynamic;

using Kephas.Dynamic;
using NUnit.Framework;

[TestFixture]
public class DictionaryExpandoTest
{
    [Test]
    public void Constructor_object_dictionary()
    {
        var dict = new Dictionary<string, object?>();
        dynamic expando = new DictionaryExpando<object?>(dict);

        expando.Hi = "there";

        Assert.AreEqual("there", dict["Hi"]);
    }

    [Test]
    public void Constructor_string_dictionary()
    {
        var dict = new Dictionary<string, string>();
        dynamic expando = new DictionaryExpando<string>(dict);

        expando.Hi = "there";

        Assert.AreEqual("there", dict["Hi"]);
    }
}