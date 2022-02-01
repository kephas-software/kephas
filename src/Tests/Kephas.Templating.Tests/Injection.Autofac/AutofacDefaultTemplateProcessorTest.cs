// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacDefaultTemplateProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Tests.Injection.Autofac
{
    using System.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class AutofacDefaultTemplateProcessorTest : AutofacTemplatingTestBase
    {
        [Test]
        public void DefaultTemplateProcessor_Injection_success()
        {
            var container = this.CreateInjector();
            var templateProcessor = container.Resolve<ITemplateProcessor>();
            Assert.IsInstanceOf<DefaultTemplateProcessor>(templateProcessor);

            var typedTemplateProcessor = (DefaultTemplateProcessor)templateProcessor;
            Assert.IsNotNull(typedTemplateProcessor.Logger);
        }

        [Test]
        public async Task ProcessAsync_Injection_success()
        {
            var container = this.CreateInjector(parts: new[] { typeof(TestTemplatingEngine) });
            var templateProcessor = container.Resolve<ITemplateProcessor>();

            var template = new StringTemplate("test-template", "test", "dummy");
            var result = await templateProcessor.ProcessAsync<object>(template);

            Assert.AreEqual("processed test-template", result);
        }
    }
}