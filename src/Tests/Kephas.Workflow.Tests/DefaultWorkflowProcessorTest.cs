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
    using Kephas.Testing;
    using Kephas.Workflow.Behaviors;
    using Kephas.Workflow.Behaviors.Composition;
    using Kephas.Workflow.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultWorkflowProcessorTest : TestBase
    {
        [Test]
        public async Task ExecuteAsync_basic_flow()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new ActivityContext(Substitute.For<ICompositionContext>(), Substitute.For<IWorkflowProcessor>()));
            var processor = new DefaultWorkflowProcessor(ctxFactory, new List<IExportFactory<IActivityBehavior, ActivityBehaviorMetadata>>());

            var activityInfo = Substitute.For<IActivityInfo>();
            var activity = Substitute.For<IActivity>();
            activity.GetTypeInfo().Returns(activityInfo);

            var target = new object();
            var expected = new object();
            activityInfo.ExecuteAsync(activity, target, Arg.Any<IExpando>(), Arg.Any<IActivityContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expected));

            var result = await processor.ExecuteAsync(activity, target, arguments: null, optionsConfig: null);
            Assert.AreSame(expected, result);
        }
    }
}