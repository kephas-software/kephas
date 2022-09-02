// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicExpandoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Dynamic;

using Kephas.Dynamic;
using NUnit.Framework;

[TestFixture]
public class DynamicExpandoTest
{
    [Test]
    public void Indexer()
    {
        dynamic obj = new DictionaryExpando<object?>(new Dictionary<string, object?> { { "name", "John Doe" } });
        var expando = new DynamicExpando(obj);

        Assert.AreEqual("John Doe", expando["name"]);
    }
}