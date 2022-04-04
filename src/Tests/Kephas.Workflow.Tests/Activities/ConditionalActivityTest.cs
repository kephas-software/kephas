// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionalActivityTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests.Activities;

using Kephas.Operations;
using Kephas.Services;
using Kephas.Workflow.Activities;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ConditionalActivityTest : FlowActivityTestBase
{
    [TestCase(true, "hi", "there", "hi")]
    [TestCase(false, "hi", "there", "there")]
    [TestCase(null, "hi", "there", "there")]
    public async Task ExecuteAsync(bool? condValue, string thenValue, string elseValue, string resultValue)
    {
        var cond = Substitute.For<IActivity, IOperation>();
        (cond as IOperation).ExecuteAsync(Arg.Any<IContext?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((object?)condValue));
        var then = Substitute.For<IActivity, IOperation>();
        (then as IOperation).ExecuteAsync(Arg.Any<IContext?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((object?)thenValue));
        var @else = Substitute.For<IActivity, IOperation>();
        (@else as IOperation).ExecuteAsync(Arg.Any<IContext?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((object?)elseValue));

        var context = this.CreateActivityContext();
        var activity = new ConditionalActivity { Condition = condValue is null ? null : cond, Then = then, Else = @else };
        var result = await activity.ExecuteAsync(context);

        Assert.AreEqual(resultValue, result);
    }

    [Test]
    public void Clone()
    {
        var cond = Substitute.For<IActivity>();
        var then = Substitute.For<IActivity, ICloneable>();
        var thenClone = Substitute.For<IActivity>();
        ((ICloneable)then).Clone().Returns(thenClone);
        var @else = Substitute.For<IActivity>();

        var activity = new ConditionalActivity { Condition = cond, Then = then, Else = @else };
        var clone = activity.Clone() as ConditionalActivity;

        Assert.AreNotSame(activity, clone);
        Assert.AreSame(cond, clone.Condition);
        Assert.AreSame(thenClone, clone.Then);
        Assert.AreSame(@else, clone.Else);
    }
}