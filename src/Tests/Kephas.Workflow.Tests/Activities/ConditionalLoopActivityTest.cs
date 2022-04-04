// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionalLoopActivityTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests.Activities;

using System.Text;
using Kephas.Operations;
using Kephas.Services;
using Kephas.Workflow.Activities;
using Kephas.Workflow.Interaction;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ConditionalLoopActivityTest : FlowActivityTestBase
{
    [TestCase(2, "hi", "hihi")]
    [TestCase(3, "hi", "hihihi")]
    public async Task ExecuteAsync(int? iterations, string iterationValue, string resultValue)
    {
        var sb = new StringBuilder();
        var cond = Substitute.For<IActivity, IOperation>();
        (cond as IOperation).ExecuteAsync(Arg.Any<IContext?>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult((object?)(iterations is not null && (iterations-- > 0))));
        var iter = Substitute.For<IActivity, IOperation>();
        (iter as IOperation).ExecuteAsync(Arg.Any<IContext?>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult((object?)sb.Append(iterationValue).ToString()));

        var context = this.CreateActivityContext();
        var activity = new ConditionalLoopActivity { Condition = iterations is null ? null : cond, Do = iter };
        var result = await activity.ExecuteAsync(context);

        Assert.AreEqual(resultValue, result);
    }

    [Test]
    public async Task ExecuteAsync_break()
    {
        var iter = Substitute.For<IActivity, IOperation>();
        (iter as IOperation).ExecuteAsync(Arg.Any<IContext?>(), Arg.Any<CancellationToken>())
            .Returns<Task<object?>>(ci => throw new BreakLoopSignal());

        var context = this.CreateActivityContext();
        var activity = new ConditionalLoopActivity { Condition = null, Do = iter };
        var result = await activity.ExecuteAsync(context);

        Assert.IsNull(result);
        (iter as IOperation).Received(1).ExecuteAsync(Arg.Any<IContext?>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public void Clone()
    {
        var cond = Substitute.For<IActivity>();
        var doActivity = Substitute.For<IActivity, ICloneable>();
        var doClone = Substitute.For<IActivity>();
        ((ICloneable)doActivity).Clone().Returns(doClone);

        var activity = new ConditionalLoopActivity { Condition = cond, Do = doActivity };
        var clone = activity.Clone() as ConditionalLoopActivity;

        Assert.AreNotSame(activity, clone);
        Assert.AreSame(cond, clone.Condition);
        Assert.AreSame(doClone, clone.Do);
    }
}
