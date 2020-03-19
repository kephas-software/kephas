// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeActivityInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime activity information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests.Runtime
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Runtime;
    using Kephas.Runtime.AttributedModel;
    using Kephas.Services;
    using Kephas.Workflow.Runtime;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class RuntimeActivityInfoTest
    {
        [Test]
        public void ReturnType_void()
        {
            var activityInfo = new RuntimeActivityInfo(typeof(ITestActivity));
            Assert.AreSame(typeof(void), (activityInfo.ReturnType as IRuntimeTypeInfo).Type);
        }

        [Test]
        public void ReturnType_explicit()
        {
            var activityInfo = new RuntimeActivityInfo(typeof(TestActivity));
            Assert.AreSame(typeof(int), (activityInfo.ReturnType as IRuntimeTypeInfo).Type);
        }

        [Test]
        public void Parameters_order()
        {
            var activityInfo = new RuntimeActivityInfo(typeof(ITestActivity));
            var paramList = activityInfo.Parameters.OrderBy(p => p.Position).ToList();

            Assert.AreEqual(6, paramList.Count);
            Assert.AreEqual("ImplicitInput", paramList[0].Name);
            Assert.AreEqual("ExplicitInput", paramList[1].Name);
            Assert.AreEqual("ImplicitRequired", paramList[2].Name);
            Assert.AreEqual("ExplicitRequired", paramList[3].Name);
            Assert.AreEqual("Output", paramList[4].Name);
            Assert.AreEqual("ByRef", paramList[5].Name);
        }

        [Test]
        public void Parameters_class_order()
        {
            var activityInfo = new RuntimeActivityInfo(typeof(TestActivity));
            var paramList = activityInfo.Parameters.OrderBy(p => p.Position).ToList();

            Assert.AreEqual(6, paramList.Count);
            Assert.AreEqual("ImplicitInput", paramList[0].Name);
            Assert.AreEqual("ExplicitInput", paramList[1].Name);
            Assert.AreEqual("ImplicitRequired", paramList[2].Name);
            Assert.AreEqual("ExplicitRequired", paramList[3].Name);
            Assert.AreEqual("Output", paramList[4].Name);
            Assert.AreEqual("ByRef", paramList[5].Name);
        }

        [Test]
        public void ExecuteAsync_not_implemented()
        {
            var activityInfo = new RuntimeActivityInfo(typeof(TestActivity));
            Assert.ThrowsAsync<NotImplementedException>(
                () => activityInfo.ExecuteAsync(new TestActivity(), null, null, new ActivityContext(Substitute.For<ICompositionContext>(), Substitute.For<IWorkflowProcessor>())));
        }

        [Test]
        public async Task ExecuteAsync_operation()
        {
            var activityInfo = new RuntimeActivityInfo(typeof(TestActivity));
            var activity = Substitute.For<ActivityBase, IOperation>();
            (activity as IOperation).Execute(Arg.Any<IContext>()).Returns("success");


            Assert.AreEqual("success", await activityInfo.ExecuteAsync(activity, null, null, new ActivityContext(Substitute.For<ICompositionContext>(), Substitute.For<IWorkflowProcessor>())));
        }

        [Test]
        public async Task ExecuteAsync_async_operation()
        {
            var activityInfo = new RuntimeActivityInfo(typeof(TestActivity));
#if NETCOREAPP3_1
            var activity = Substitute.For<ActivityBase, IOperation>();

            (activity as IOperation).ExecuteAsync(Arg.Any<IContext>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult<object>("success"));
#else
            var activity = Substitute.For<ActivityBase, IAsyncOperation>();
            (activity as IAsyncOperation).ExecuteAsync(Arg.Any<IContext>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult<object>("success"));
#endif

            Assert.AreEqual("success", await activityInfo.ExecuteAsync(activity, null, null, new ActivityContext(Substitute.For<ICompositionContext>(), Substitute.For<IWorkflowProcessor>())));
        }

        public interface ITestActivity : IActivity
        {
            string ImplicitInput { get; set; }

            [In]
            int ExplicitInput { get; set; }

            bool ImplicitRequired { get; set; }

            [Required]
            bool? ExplicitRequired { get; set; }

            [Out]
            object Output { get; set; }

            [In, Out]
            decimal ByRef { get; set; }
        }

        [ReturnType(typeof(int))]
        public class TestActivity : ActivityBase
        {
            public string ImplicitInput { get; set; }

            [In]
            public int ExplicitInput { get; set; }

            public bool ImplicitRequired { get; set; }

            [Required]
            public bool? ExplicitRequired { get; set; }

            [Out]
            public object Output { get; set; }

            [In, Out]
            public decimal ByRef { get; set; }
        }
    }
}