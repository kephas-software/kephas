// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SequenceActivityTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests.Activities;

using System.Text;

using Kephas.Operations;
using Kephas.Services;
using Kephas.Workflow.Activities;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class SequenceActivityTest : FlowActivityTestBase
{
    [Test]
    public async Task ExecuteAsync()
    {
        var sb = new StringBuilder();
        var a1 = Substitute.For<IActivity, IOperation>();
        (a1 as IOperation).ExecuteAsync(Arg.Any<IContext?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((object?)sb.Append("hi").ToString()));
        var a2 = Substitute.For<IActivity, IOperation>();
        (a2 as IOperation).ExecuteAsync(Arg.Any<IContext?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((object?)sb.Append(" there").ToString()));

        var context = this.CreateActivityContext();
        var activity = new SequenceActivity { Activities = new[] { a1, a2 } };
        var result = await activity.ExecuteAsync(context);

        Assert.AreEqual("hi there", result);
    }

    [Test]
    public void Clone()
    {
        var a1 = Substitute.For<IActivity>();
        var a2 = Substitute.For<IActivity, ICloneable>();
        var a2clone = Substitute.For<IActivity>();
        ((ICloneable)a2).Clone().Returns(a2clone);

        var activity = new SequenceActivity { Activities = new[] { a1, a2 } };
        var clone = activity.Clone() as SequenceActivity;

        Assert.AreNotSame(activity, clone);
        Assert.AreSame(a1, clone.Activities.First());
        Assert.AreSame(a2clone, clone.Activities.Skip(1).First());
    }
}