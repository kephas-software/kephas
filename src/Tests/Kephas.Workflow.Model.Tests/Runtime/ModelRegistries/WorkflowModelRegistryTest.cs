// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowModelRegistryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model.Tests.Runtime.ModelRegistries
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Logging;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Runtime;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Runtime.AttributedModel;
    using Kephas.Testing;
    using Kephas.Workflow.Model.Runtime.ModelRegistries;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class WorkflowModelRegistryTest : TestBase
    {
        private RuntimeTypeRegistry typeRegistry;

        public WorkflowModelRegistryTest()
        {
            this.typeRegistry = new RuntimeTypeRegistry();
        }

        [Test]
        public async Task GetRuntimeElementsAsync()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime
                .GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { this.GetType().Assembly });

            var typeLoader = Substitute.For<ITypeLoader>();
            typeLoader.GetExportedTypes(Arg.Any<Assembly>()).Returns(new[] { typeof(IActivity), typeof(IActivityType), typeof(IStateMachine), typeof(IStateMachineType), typeof(ActivityBase), typeof(string), typeof(TestActivity), typeof(TestStateMachine) });

            var contextFactory = this.CreateContextFactoryMock(() =>
                new ModelRegistryConventions(Substitute.For<ICompositionContext>()));

            var registry = new WorkflowModelRegistry(contextFactory, appRuntime, typeLoader);
            var result = (await registry.GetRuntimeElementsAsync()).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.AreSame(typeof(TestActivity), result[0]);
            Assert.AreSame(typeof(TestStateMachine), result[1]);
        }

        [Test]
        public async Task GetRuntimeElementsAsync_ExcludeFromModel()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime
                .GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { this.GetType().Assembly });

            var typeLoader = Substitute.For<ITypeLoader>();
            typeLoader.GetExportedTypes(Arg.Any<Assembly>()).Returns(new[] { typeof(IActivity), typeof(IActivityType), typeof(ActivityBase), typeof(string), typeof(ExcludedActivity) });

            var contextFactory = this.CreateContextFactoryMock(() =>
                new ModelRegistryConventions(Substitute.For<ICompositionContext>()));

            var registry = new WorkflowModelRegistry(contextFactory, appRuntime, typeLoader);
            var result = (await registry.GetRuntimeElementsAsync()).ToList();
            Assert.AreEqual(0, result.Count);
        }

        [ExcludeFromModel]
        public class ExcludedActivity : ActivityBase, IActivity
        {
        }

        [ReturnType(typeof(int))]
        public class TestActivity : ActivityBase
        {
        }

        [ReturnType(typeof(int))]
        public class TestStateMachine : StateMachineBase<string, LogLevel>
        {
            public TestStateMachine(string target, IRuntimeTypeRegistry typeRegistry, ILogManager? logManager = null)
                : base(target, typeRegistry, logManager)
            {
            }
        }
    }
}