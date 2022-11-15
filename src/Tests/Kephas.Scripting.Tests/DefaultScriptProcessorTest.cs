// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultScriptProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Tests
{
    using System.Threading.Tasks;
    using Kephas.Testing;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultScriptProcessorTest : ScriptingTestBase
    {
        [Test]
        public void DefaultScriptProcessor_Injection_success()
        {
            var container = this.CreateServicesBuilder()
                .BuildWithDependencyInjection();
            var scriptProcessor = container.Resolve<IScriptProcessor>();
            Assert.IsInstanceOf<DefaultScriptProcessor>(scriptProcessor);

            var typedScriptingProcessor = (DefaultScriptProcessor)scriptProcessor;
            Assert.IsNotNull(typedScriptingProcessor.Logger);
        }

        [Test]
        public async Task ExecuteAsync_Injection_success()
        {
            var container = this.CreateServicesBuilder()
                .WithParts(typeof(TestLanguageService))
                .BuildWithDependencyInjection();
            var scriptingEngine = container.Resolve<IScriptProcessor>();

            var script = new StringScript("dummy", "test");
            var result = await scriptingEngine.ExecuteAsync(script);

            Assert.AreEqual("executed dummy", result);
        }
    }
}