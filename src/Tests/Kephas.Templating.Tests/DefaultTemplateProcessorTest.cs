// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTemplateProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Tests
{
    using System.Text;
    using System.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultTemplateProcessorTest : TemplatingTestBase
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

            var template = new StringTemplate("dummy", "test", "test-template");
            var result = await templateProcessor.ProcessAsync<object>(template);

            Assert.AreEqual("processed test-template", result);
        }

        [Test]
        public async Task ProcessAsync_Injection_failure_missing_handler()
        {
            var container = this.CreateInjector();
            var templateProcessor = container.Resolve<ITemplateProcessor>();

            var template = new StringBuilderTemplate(new StringBuilder("dummy"), "test", "test-template");
            Assert.ThrowsAsync<Exception>(() => templateProcessor.ProcessAsync<object>(template));
        }
    }
}