// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstActivityTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests.Activities;

using Kephas.Workflow.Activities;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ConstActivityTest : FlowActivityTestBase
{
    [Test]
    public async Task ExecuteAsync()
    {
        var context = this.CreateActivityContext();
        var activity = new ConstActivity { Value = 12 };
        var result = await activity.ExecuteAsync(context);

        Assert.AreEqual(12, result);
    }

    [Test]
    public void Clone()
    {
        var a1 = Substitute.For<IActivity>();
        var a2 = Substitute.For<IActivity, ICloneable>();
        var a2clone = Substitute.For<IActivity>();
        ((ICloneable)a2).Clone().Returns(a2clone);

        var activity = new ConstActivity { Value = 12};
        var clone = activity.Clone() as ConstActivity;

        Assert.AreNotSame(activity, clone);
        Assert.AreEqual(12, clone.Value);
    }
}