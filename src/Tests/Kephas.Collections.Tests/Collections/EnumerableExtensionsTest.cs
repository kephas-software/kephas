// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Collections;

using System.Text;
using Kephas.Collections;
using NUnit.Framework;

[TestFixture]
public class EnumerableExtensionsTest
{
    [Test]
    public void ForEach_sync()
    {
        var sb = new StringBuilder();
        new[] { 1, 2, 3 }.ForEach(v => sb.Append(v));

        Assert.AreEqual("123", sb.ToString());
    }
}