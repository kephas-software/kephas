// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeActivityParameterInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime activity parameter information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Tests.Runtime
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Runtime.AttributedModel;
    using Kephas.Workflow.Runtime;

    using NUnit.Framework;

    [TestFixture]
    public class RuntimeActivityParameterInfoTest
    {
        [Test]
        public void IsIn_implicit()
        {
            var paramInfo = new RuntimeActivityParameterInfo(
                typeof(ITestActivity).GetProperty(nameof(ITestActivity.ImplicitInput)), 2);

            Assert.IsTrue(paramInfo.IsIn);
            Assert.IsFalse(paramInfo.IsOut);
        }

        [Test]
        public void IsIn_explicit()
        {
            var paramInfo = new RuntimeActivityParameterInfo(
                typeof(ITestActivity).GetProperty(nameof(ITestActivity.ExplicitInput)), 2);

            Assert.IsTrue(paramInfo.IsIn);
            Assert.IsFalse(paramInfo.IsOut);
        }

        [Test]
        public void IsOut_explicit()
        {
            var paramInfo = new RuntimeActivityParameterInfo(
                typeof(ITestActivity).GetProperty(nameof(ITestActivity.Output)), 2);

            Assert.IsFalse(paramInfo.IsIn);
            Assert.IsTrue(paramInfo.IsOut);
        }

        [Test]
        public void IsByRef()
        {
            var paramInfo = new RuntimeActivityParameterInfo(
                typeof(ITestActivity).GetProperty(nameof(ITestActivity.ByRef)), 2);

            Assert.IsTrue(paramInfo.IsIn);
            Assert.IsTrue(paramInfo.IsOut);
        }

        [Test]
        public void IsOptional_implicit_optional()
        {
            var paramInfo = new RuntimeActivityParameterInfo(
                typeof(ITestActivity).GetProperty(nameof(ITestActivity.ImplicitInput)), 2);

            Assert.IsTrue(paramInfo.IsOptional);
        }

        [Test]
        public void IsOptional_implicit_required()
        {
            var paramInfo = new RuntimeActivityParameterInfo(
                typeof(ITestActivity).GetProperty(nameof(ITestActivity.ImplicitRequired)), 2);

            Assert.IsFalse(paramInfo.IsOptional);
        }

        [Test]
        public void IsOptional_explicit_required()
        {
            var paramInfo = new RuntimeActivityParameterInfo(
                typeof(ITestActivity).GetProperty(nameof(ITestActivity.ExplicitRequired)), 12);

            Assert.IsFalse(paramInfo.IsOptional);
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
    }
}