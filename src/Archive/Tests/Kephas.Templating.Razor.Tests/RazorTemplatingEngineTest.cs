// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorTemplatingEngineTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor.Tests;

using NUnit.Framework;

[TestFixture]
public class RazorTemplatingEngineTest : RazorTemplatingTestBase
{
    [Test]
    public void Injection()
    {
        var container = this.BuildServiceProvider();
        var engine = container.Resolve<ITemplatingEngine>("Razor");
        Assert.IsInstanceOf<RazorTemplatingEngine>(engine);
    }
}