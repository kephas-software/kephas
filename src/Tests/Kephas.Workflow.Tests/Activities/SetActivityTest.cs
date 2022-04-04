// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetActivityTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests.Activities;

using Kephas.Workflow.Activities;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class SetActivityTest : FlowActivityTestBase
{
    [Test]
    public async Task ExecuteAsync()
    {
        var context = this.CreateActivityContext();
        var activity = new SetActivity
        {
            Variable = "myvar",
            Expression = new ConstActivity { Value = "hi there" },
        };
        var result = await activity.ExecuteAsync(context);

        Assert.AreEqual("hi there", result);
        Assert.AreEqual("hi there", context.Scope["myvar"]);
    }

    [Test]
    public void Clone()
    {
        var activity = new SetActivity
        {
            Variable = "myvar",
            Expression = new ConstActivity { Value = "hi there" },
        };
        var clone = activity.Clone() as SetActivity;

        Assert.AreNotSame(activity, clone);
        Assert.AreEqual("myvar", clone.Variable);
        Assert.IsInstanceOf<ConstActivity>(clone.Expression);
        Assert.AreEqual("hi there", clone.Expression.Value);
    }
}