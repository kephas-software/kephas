// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultWorkflowProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default workflow processor test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Workflow.Behaviors;
    using Kephas.Workflow.Behaviors.Composition;
    using Kephas.Workflow.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultWorkflowProcessorTest
    {
        [Test]
        public async Task ExecuteAsync_basic_flow()
        {
            var processor = new DefaultWorkflowProcessor(new List<IExportFactory<IActivityBehavior, ActivityBehaviorMetadata>>());

            var activityInfo = Substitute.For<IActivityInfo>();
            var activity = Substitute.For<IActivity>();
            activity.GetTypeInfo().Returns(activityInfo);

            var target = new object();
            var context = Substitute.For<IActivityContext>();
            var expected = new object();
            activityInfo.ExecuteAsync(activity, target, Arg.Any<IExpando>(), context, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expected));

            var result = await processor.ExecuteAsync(activity, target, null, context);
            Assert.AreSame(expected, result);
        }
    }
}