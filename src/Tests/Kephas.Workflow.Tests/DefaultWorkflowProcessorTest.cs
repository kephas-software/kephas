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

    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using Kephas.Runtime;
    using Kephas.Testing;
    using Kephas.Testing.Services;
    using Kephas.Workflow.Behaviors;
    using Kephas.Workflow.Reflection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultWorkflowProcessorTest : TestBase
    {
        [Test]
        public void Injection()
        {
            var container = this.CreateServicesBuilder()
                .WithAssemblies(typeof(IWorkflowProcessor).Assembly, typeof(DefaultWorkflowProcessor).Assembly)
                .BuildWithDependencyInjection();
            var workflowProcessor = container.Resolve<IWorkflowProcessor>();
            Assert.IsInstanceOf<DefaultWorkflowProcessor>(workflowProcessor);
        }

        [Test]
        public async Task ExecuteAsync_basic_flow()
        {
            var ctxFactory = this.CreateInjectableFactoryMock(() => new ActivityContext(Substitute.For<IServiceProvider>(), Substitute.For<IWorkflowProcessor>()));
            var processor = new DefaultWorkflowProcessor(ctxFactory, new List<IExportFactory<IActivityBehavior, ActivityBehaviorMetadata>>(), new RuntimeTypeRegistry());

            var activityInfo = Substitute.For<IActivityInfo>();
            var activity = Substitute.For<IActivity>();
            activity.GetTypeInfo().Returns(activityInfo);

            var target = new object();
            var expected = new object();
            activityInfo.ExecuteAsync(activity, target, Arg.Any<IDynamic>(), Arg.Any<IActivityContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expected));

            var result = await processor.ExecuteAsync(activity, target, arguments: null, optionsConfig: null);
            Assert.AreSame(expected, result);
        }
    }
}