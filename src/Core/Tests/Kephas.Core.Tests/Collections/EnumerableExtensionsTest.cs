// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Kephas.Collections;
using Xunit;

namespace Kephas.Tests.Collections;

public class EnumerableExtensionsTest
{
    [Fact]
    public void ForEach_sync()
    {
        var sb = new StringBuilder();
        new[] { 1, 2, 3 }.ForEach(v => sb.Append(v));

        Assert.Equal("123", sb.ToString());
    }
}
